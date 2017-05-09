#region Using Statements
using MultiplayerTopDownTank.Behaviors;
using MultiplayerTopDownTank.Components;
using MultiplayerTopDownTank.Entities;
using System;
using WaveEngine.Common.Math;
using WaveEngine.Common.Media;
using WaveEngine.Common.Physics2D;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Framework;
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

        private readonly NetworkService networkService;
        private readonly NetworkManager networkManager;

        public GameScene(int playerIndex)
        {
            this.playerIndex = playerIndex;
            this.networkService = WaveServices.GetService<NetworkService>();
            this.networkManager = this.networkService.RegisterScene(this, GameSceneIdentifier);
        }

        protected override void CreateScene()
        {
            this.Load(WaveContent.Scenes.GameScene);

            this.CreateUI();
        }

        protected override void Start()
        {
            base.Start();

            this.SetCameraBounds();
            this.CreatePhysicScene();
            this.CreateBackgroundMusic();
            this.InitializePlayer();
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
                .AddComponent(new RectangleCollider2D())
                .AddComponent(new NetworkBehavior())
                .AddComponent(new TankNetworkSyncComponent());

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
                colliderEntity.Tag = GameConstants.TagCollider;
                colliderEntity.AddComponent(new RigidBody2D() { PhysicBodyType = RigidBodyType2D.Static });

                var collider = colliderEntity.FindComponent<Collider2D>(false);
                if (collider != null)
                {
                    collider.CollisionCategories = ColliderCategory2D.Cat3;
                    collider.CollidesWith = ColliderCategory2D.All;
                    collider.Friction = 1.0f;
                    collider.Restitution = 0.2f;
                }

                this.EntityManager.Add(colliderEntity);
            }
        }

        private void CreateBackgroundMusic()
        {
            // Music
            var musicInfo = new MusicInfo(WaveContent.Assets.Sounds.Background_Music_mp3);
            WaveServices.MusicPlayer.Play(musicInfo);
            WaveServices.MusicPlayer.Volume = 0.8f;
            WaveServices.MusicPlayer.IsRepeat = true;
        }
    }
}