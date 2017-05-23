#region Using Statements
using MultiplayerTopDownTank.Behaviors;
using MultiplayerTopDownTank.Components;
using MultiplayerTopDownTank.Entities;
using MultiplayerTopDownTank.Managers;
using System;
using System.Collections.Generic;
using WaveEngine.Common.Math;
using WaveEngine.Common.Media;
using WaveEngine.Common.Physics2D;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Framework;
using WaveEngine.Framework.Diagnostic;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Managers;
using WaveEngine.Framework.Physics2D;
using WaveEngine.Framework.Services;
using WaveEngine.Networking;
using WaveEngine.TiledMap;
#endregion

namespace MultiplayerTopDownTank
{
    public class GameScene : Scene
    {
        private const string GameSceneIdentifier = "MultiplayerTopDownTank.Game.Scene";

        private TiledMap tiledMap;
        private int playerIndex;
        private Entity playerEntity;
        private Entity managerEntity;
        private BulletManager bulletManager;

        private readonly NetworkService networkService;
        private readonly NetworkManager networkManager;

        public GameScene(int playerIndex)
        {
            this.playerIndex = playerIndex;
            this.networkService = WaveServices.GetService<NetworkService>();
            this.networkManager = this.networkService.RegisterScene(this, GameSceneIdentifier);
            Labels.Add("PlayerIndex", playerIndex);
        }

        protected override void CreateScene()
        {
            this.Load(WaveContent.Scenes.GameScene);

#if DEBUG
            this.EntityManager.Add(new Entity().AddComponent(new DebugBehavior()));
#endif

            this.CreateUI();
        }

        protected override void Start()
        {
            base.Start();

            this.SetCameraBounds();
            this.ConfigurePhysics();
            this.CreatePhysicScene();
            this.CreateBackgroundMusic();
            this.CreateManager();
            this.InitializePlayer();
        }

        private void CreateManager()
        {
            bulletManager = new BulletManager
            {
                BulletPoolSize = GameConstants.BulletPoolSize
            };

            this.managerEntity = new Entity(GameConstants.Manager)
                .AddComponent(bulletManager);

            this.EntityManager.Add(managerEntity);
        }

        private void CreatePlayer(Vector2 position, string name)
        {
            this.playerEntity = new Entity(name)
                .AddComponent(new Transform2D
                {
                    Position = position,
                    Origin = new Vector2(0.5f, 0.5f)
                })
                .AddComponent(new Sprite
                {
                    TexturePath = string.Format("Content/Assets/Textures/Tanks/tankBase{0}.png", playerIndex)
                })
                .AddComponent(new SpriteRenderer(DefaultLayers.Alpha))
                .AddComponent(new TankBehavior())
                .AddComponent(new TankComponent())
                .AddComponent(new BulletEmitter())
                .AddComponent(new RectangleCollider2D
                {
                    CollisionCategories = ColliderCategory2D.Cat1,
                    CollidesWith = ColliderCategory2D.Cat1 | ColliderCategory2D.Cat3 | ColliderCategory2D.Cat4
                })
                .AddComponent(new NetworkBehavior())
                .AddComponent(new TankNetworkSyncComponent())
                .AddComponent(new RigidBody2D
                {
                    AngularDamping = 5.0f,
                    LinearDamping = 10.0f,
                });

            var barrel = new Entity(GameConstants.PlayerBarrel)
              .AddComponent(new Transform2D
              {
                  LocalDrawOrder = -0.1f,
                  Origin = new Vector2(0.5f, 0.75f)
              })
              .AddComponent(new Sprite
              {
                  TexturePath = string.Format("Content/Assets/Textures/Tanks/tankBarrel{0}.png", playerIndex)
              })
              .AddComponent(new SpriteRenderer(DefaultLayers.Alpha))
              .AddComponent(new NetworkBehavior())
              .AddComponent(new TankBarrelNetworkSyncComponent());

            playerEntity.AddChild(barrel);

            this.EntityManager.Add(playerEntity);
        }

        private Vector2 GetRandomSpawnPoint()
        {
            var anchorLayer = tiledMap.ObjectLayers[GameConstants.AnchorLayer];
            WaveEngine.Framework.Services.Random rnd = new WaveEngine.Framework.Services.Random();
            var spawnIndex = rnd.Next(1, Convert.ToInt32(tiledMap.Properties[GameConstants.SpawnCount]));
            var position = anchorLayer.Objects.Find(o => o.Name == $"{GameConstants.SpawnPointPrefix}{spawnIndex}");

            return new Vector2(position.X, position.Y);
        }

        private void InitializePlayer()
        {
            this.CreatePlayer(GetRandomSpawnPoint(), "Player_" + this.playerIndex);

            // When the scene start add the payer entity to NetworkManager to start to sync with other clients.
            this.networkManager.AddEntity(playerEntity);

            var playerComponent = playerEntity.FindComponent<TankComponent>();
            playerComponent.PrepareTank();
        }

        private void SetCameraBounds()
        {
            var cameraBehavior = this.RenderManager.ActiveCamera2D.Owner.FindComponent<CameraBehavior>(false);
            if (cameraBehavior != null)
            {
                this.tiledMap = this.EntityManager.Find(GameConstants.EntityMap).FindComponent<TiledMap>();
                cameraBehavior.SetLimits(
                    new Vector2(0, 0),
                    new Vector2(tiledMap.Width * tiledMap.TileWidth, tiledMap.Height * tiledMap.TileHeight));
            }
        }

        private void ConfigurePhysics()
        {
            this.PhysicsManager.Simulation2D.Gravity = Vector2.Zero;
        }

        private void CreateUI()
        {
            // Left Joystick
            RectangleF leftArea = new RectangleF(
                0,
                0,
                this.VirtualScreenManager.VirtualWidth / 2f,
                this.VirtualScreenManager.VirtualHeight);
            var leftJoystick = new Joystick("leftJoystick", leftArea);
            EntityManager.Add(leftJoystick);

            // Right Joystick
            RectangleF rightArea = new RectangleF(
                this.VirtualScreenManager.VirtualWidth / 2,
                0,
                this.VirtualScreenManager.VirtualWidth / 2f,
                this.VirtualScreenManager.VirtualHeight);
            var rightJoystick = new Joystick("rightJoystick", rightArea);
            EntityManager.Add(rightJoystick);

            CreateHub();
        }

        private void CreateHub()
        {
            // Create Hub
            var hubPanel = new Hub();
            EntityManager.Add(hubPanel);
        }

        private void CreatePhysicScene()
        {
            // Invisible physic walls
            var physicLayer = this.tiledMap.ObjectLayers[GameConstants.PhysicLayer];
            foreach (var physic in physicLayer.Objects)
            {
                var colliderEntity = TiledMapUtils.CollisionEntityFromObject(physic.Name, physic);
                colliderEntity.Name = GameConstants.MapColliderEntity;
                colliderEntity.Tag = GameConstants.TagCollider;
                colliderEntity.AddComponent(new RigidBody2D() { PhysicBodyType = RigidBodyType2D.Static });

                var collider2D = colliderEntity.FindComponent<Collider2D>(false);
                collider2D.CollisionCategories = ColliderCategory2D.Cat4;
                collider2D.CollidesWith = ColliderCategory2D.All;

                collider2D.BeginCollision += (args) =>
                {
                    if(args.ColliderB != null && args.ColliderB.UserData is Collider2D)
                    {
                        var tag = ((Collider2D)args.ColliderB.UserData).Owner.Tag;

                        if(tag != null && tag.Equals(GameConstants.BulletTag))
                        {
                            var bullets = new List<Entity>
                            {
                                ((Collider2D)args.ColliderB.UserData).Owner
                            };

                            bulletManager.FreeBulletEntity(bullets);
                        }
                    }
                };
                
                this.EntityManager.Add(colliderEntity);
            }
        }

        private void CreateBackgroundMusic()
        {
            // Music
            var musicInfo = new MusicInfo(WaveContent.Assets.Sounds.Background_Music_mp3);
            WaveServices.MusicPlayer.Play(musicInfo);
            WaveServices.MusicPlayer.Volume = 0.0f;
            WaveServices.MusicPlayer.IsRepeat = true;
        }
    }
}