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
using WaveEngine.Framework.Diagnostic;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Managers;
using WaveEngine.Framework.Physics2D;
using WaveEngine.Framework.Services;
using WaveEngine.Networking;
using WaveEngine.TiledMap;
using WaveEngine.Networking.Messages;
using MultiplayerTopDownTank.Managers;
using MultiplayerTopDownTank.Messages;
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
        private NetworkSceneSyncBehavior networkSceneSyncBehavior;

        public GameScene(int playerIndex)
        {
            this.playerIndex = playerIndex;
            this.networkService = WaveServices.GetService<NetworkService>();
            this.networkManager = this.networkService.RegisterScene(this, GameSceneIdentifier);
            this.networkManager.AddFactory(GameConstants.TankFactory, OnPlayerReceived);
            this.networkManager.AddFactory(GameConstants.BulletFactory, OnBulletReceived);
        }

        public NetworkManager NetworkManager { get { return this.networkManager; } }

        public void Shoot(Vector2 position, Vector2 direction)
        {
            var bullet = this.networkManager.AddEntity(GameConstants.BulletFactory);

            bullet.FindComponent<BulletComponent>().Shoot(position, direction);
        }

        private Entity OnPlayerReceived(string clientId, string behaviorId)
        {
            return this.CreateNetworkTank(clientId);
        }

        private Entity OnBulletReceived(string clientId, string behaviorId)
        {
            return this.CreateBullet();
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

            this.InitializeSceneBehaviors();
            this.SetCameraBounds();
            this.ConfigurePhysics();
            this.CreatePhysicScene();
            this.CreateBackgroundMusic();
            this.InitializePlayer();
            this.InitializeBullet();

            this.networkService.MessageReceivedFromHost += this.ClientMessageReceived;
            this.networkService.MessageReceivedFromClient += this.HostMessageReceived;
        }

        private void HostMessageReceived(object sender, NetworkEndpoint fromEndpoint, IncomingMessage receivedMessage)
        {
   
        }

        private void ClientMessageReceived(object sender, NetworkEndpoint fromEndpoint, IncomingMessage receivedMessage)
        {
            NetworkCommandEnum command;
            string playerName;
            string emptyParameter;

            NetworkMessageHelper.ReadMessage(receivedMessage, out command, out playerName, out emptyParameter);

            switch (command)
            {
                case NetworkCommandEnum.CreatePlayer:
                    break;
                case NetworkCommandEnum.Die:
                    this.NetworkSyncDie(playerName);
                    break;
            }
        }

        private void NetworkSyncDie(string playerName)
        {
            if (playerName == this.playerEntity.Name)
            {
                this.networkService.Disconnect();
                var navigationManager = WaveServices.GetService<NavigationManager>();
                navigationManager.InitialNavigation();
            }
        }

        private void InitializeSceneBehaviors()
        {
            this.networkSceneSyncBehavior = new NetworkSceneSyncBehavior();
            this.AddSceneBehavior(this.networkSceneSyncBehavior, SceneBehavior.Order.PreUpdate);
        }

        protected override void End()
        {
            base.End();
            this.RemoveSceneBehavior(this.networkSceneSyncBehavior);

            this.networkService.MessageReceivedFromHost -= this.ClientMessageReceived;
            this.networkService.MessageReceivedFromClient -= this.HostMessageReceived;
        }

        private Entity CreateTank(Vector2 position, string name, bool isLocal)
        {
            var tankCollider = new RectangleCollider2D
            {
                CollisionCategories = ColliderCategory2D.Cat1,
                CollidesWith = isLocal ?
                        ColliderCategory2D.Cat1 | ColliderCategory2D.Cat3 | ColliderCategory2D.Cat4 :
                        ColliderCategory2D.All
            };

            var tankComponent = new TankComponent();
            var tankBehavior = new TankBehavior();

            this.playerEntity = new Entity(name)
                //string.Format("{0}-{1}", isLocal ? "Local" : "Remote", name))
            {
                Tag = GameConstants.TankTag
            }
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
            .AddComponent(tankBehavior)
            .AddComponent(tankComponent)
            .AddComponent(tankCollider)
            .AddComponent(new NetworkBehavior())
            .AddComponent(new TankNetworkSyncComponent())
            .AddComponent(new RigidBody2D
            {
                AngularDamping = 5.0f,
                LinearDamping = 10.0f,
            });

            Labels.Add("PlayerEntity Name", this.playerEntity.Name);

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
              .AddComponent(new SpriteRenderer(DefaultLayers.Alpha));

            playerEntity.AddChild(barrel);

            return playerEntity;
        }


        private Entity CreateNetworkTank(string clientId)
        {
            var isHostPlayer = this.networkService.ClientIdentifier == clientId;

            if (isHostPlayer)
            {
                var randomSpawnPoint = GetRandomSpawnPoint();
                return CreateTank(randomSpawnPoint, clientId, true);
            }
            else
            {
                var enemy = CreateTank(new Vector2(-500, -500), clientId, false);
                RemoveTankBehavior(enemy);

                return enemy;
            }
        }

        private Entity CreateBullet()
        {
            var bulletCollider = new CircleCollider2D
            {
                CollisionCategories = ColliderCategory2D.Cat2,
                CollidesWith =
                       ColliderCategory2D.Cat1 |
                       ColliderCategory2D.Cat3 |
                       ColliderCategory2D.Cat4
            };

            var bullet = new Entity() { Tag = GameConstants.BulletTag }
               .AddComponent(new Transform2D
               {
                   Origin = Vector2.Center,
                   X = 0,
                   Y = 0,
                   DrawOrder = 0.6f,
               })
               .AddComponent(new RigidBody2D
               {
                   PhysicBodyType = RigidBodyType2D.Dynamic,
                   IsBullet = true,
                   LinearDamping = 0
               })
               .AddComponent(bulletCollider)
               .AddComponent(new Sprite(WaveContent.Assets.Textures.Bullets.rounded_bulletBeige_outline_png))
               .AddComponent(new SpriteRenderer(DefaultLayers.Alpha))
               .AddComponent(new BulletComponent())
               .AddComponent(new NetworkBehavior())
               .AddComponent(new BulletNetworkSyncComponent());

            bulletCollider.BeginCollision += (args) =>
            {
                this.networkSceneSyncBehavior.AddEntityToRemove(bullet);

                if (args.ColliderA != null && args.ColliderA.UserData is Collider2D)
                {
                    var tankTag = ((Collider2D)args.ColliderA.UserData).Owner.Tag;
                    if (tankTag != null && tankTag.Equals(GameConstants.TankTag))
                    {
                        var tank = ((Collider2D)args.ColliderA.UserData).Owner;
                        this.networkSceneSyncBehavior.DamageTank(tank);
                    }
                }
            };

            return bullet;
        }

        private void RemoveTankBehavior(Entity tank)
        {
            tank.RemoveComponent<TankBehavior>();
        }

        private Vector2 GetRandomSpawnPoint()
        {
            var anchorLayer = tiledMap.ObjectLayers[GameConstants.AnchorLayer];
            WaveEngine.Framework.Services.Random rnd = new WaveEngine.Framework.Services.Random();
            var spawnIndex = rnd.Next(1, Convert.ToInt32(tiledMap.Properties[GameConstants.SpawnCount]));
#if DEBUG
            spawnIndex = 1;
#endif
            var position = anchorLayer.Objects.Find(o => o.Name == $"{GameConstants.SpawnPointPrefix}{spawnIndex}");
            return new Vector2(position.X, position.Y);
        }

        private void InitializePlayer()
        {
            // When the scene start add the payer entity to NetworkManager to start to sync with other clients.
            this.networkManager.AddEntity(GameConstants.TankFactory);

            var playerComponent = playerEntity.FindComponent<TankComponent>();
            playerComponent.PrepareTank();
        }

        private void InitializeBullet()
        {
            this.networkManager.AddEntity(GameConstants.BulletFactory);
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
            var hud = new Hub(GameConstants.Hud);
            EntityManager.Add(hud);
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

                /*
                collider2D.BeginCollision += (args) =>
                {
                    if (args.ColliderB != null && args.ColliderB.UserData is Collider2D)
                    {
                        var tag = ((Collider2D)args.ColliderB.UserData).Owner.Tag;

                        if (tag != null && tag.Equals(GameConstants.BulletTag))
                        {
                            var bullet = ((Collider2D)args.ColliderB.UserData).Owner;

                            this.networkManager.RemoveEntity(bullet);
                        }
                    }
                };
                */

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