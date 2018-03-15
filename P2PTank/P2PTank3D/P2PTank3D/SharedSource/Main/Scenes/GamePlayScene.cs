using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using P2PTank.Behaviors;
using P2PTank.Behaviors.Cameras;
using P2PTank.Entities.P2PMessages;
using P2PTank.Managers;
using WaveEngine.Common.Math;
using WaveEngine.Common.Physics2D;
using WaveEngine.Framework;
using WaveEngine.Framework.Diagnostic;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Physics2D;
using WaveEngine.Components.UI;
using WaveEngine.Framework.UI;
using WaveEngine.Components.GameActions;
using WaveEngine.Common.Graphics;
using WaveEngine.Framework.Services;
using P2PTank.Services;
using P2PTank.Components;
using P2PTank.Managers.P2PMessages;
using P2PTank.Tools;
using Networking.P2P.TransportLayer;
using Networking.P2P.TransportLayer.EventArgs;
using P2PTank3D;
using P2PTank3D.Services;

namespace P2PTank.Scenes
{
    public enum PowerUpType
    {
        Bullet,
        Repair
    }

    public class GamePlayScene : Scene
    {
        private List<string> activeBullets = new List<string>();

        private List<PeerPlayer> ConnectedPeers { get; set; } = new List<PeerPlayer>();

        private MapLoader mapLoader;
        private P2PManager peerManager;
        private GamePlayManager gameplayManager;
        private PowerUpManager powerUpManager;

        private string playerID;

        public GamePlayScene()
        {
            var localHostService = WaveServices.GetService<LocalhostService>();

            this.mapLoader = new MapLoader();
            this.peerManager = new P2PManager(8080, "Ethernet");

            this.playerID = Guid.NewGuid().ToString();

            this.peerManager.PeerPlayerChange += this.OnPeerChanged;
            this.peerManager.MsgReceived += this.OnMsgReceived;
        }

        protected override async void CreateScene()
        {
            this.Load(WaveContent.Scenes.LevelBaseScene);

            this.mapLoader.Load(
                WaveContent.Assets.Maps.level1_tmap,
                WaveContent.Assets.Models.Materials.wallMaterial,
                WaveContent.Assets.Models.Materials.floorMaterial,
                this.EntityManager).Wait();

            this.UpdateMiniMapPosition();

            var audioService = WaveServices.GetService<AudioService>();
            audioService.Play(Audio.Music.Background_mp3, 0.4f);

#if DEBUG
            var debugEntity = new Entity()
                .AddComponent(new DebugBehavior());
            this.EntityManager.Add(debugEntity);
#endif

            await peerManager.StartAsync();
        }

        public void UpdateMiniMapPosition()
        {
            this.EntityManager.Find("camera2D")
              .FindComponent<Transform2D>().Position = new Vector2
                  (GameConstants.MiniMapScale * (VirtualScreenManager.ScreenWidth / 2) - GameConstants.MiniMapMargin,
                  GameConstants.MiniMapScale * (VirtualScreenManager.ScreenHeight / 2) - GameConstants.MiniMapMargin);
        }

        public void CreateDebugPowerUp()
        {
            this.powerUpManager.EmptyTimeCounter();
        }

        public void CreateCountDown()
        {
            Vector2 pos = new Vector2(VirtualScreenManager.ScreenWidth / 2, VirtualScreenManager.ScreenHeight / 2);
            VirtualScreenManager.ToVirtualPosition(ref pos);

            var entity = new Entity()
                .AddComponent(new Transform2D()
                {
                    Position = pos,
                    LocalScale = new Vector2(1),
                    Origin = Vector2.Center,
                });
            var countDownTextBlock = new TextBlock()
            {
                FontPath = WaveContent.Assets.Fonts.Top_Secret_36_ttf,
                HorizontalAlignment = HorizontalAlignment.Center,
                Foreground = Color.DarkOliveGreen,
                VerticalAlignment = VerticalAlignment.Center,
            };

            var grid = new Grid()
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
            };
            grid.Add(countDownTextBlock);

            entity.AddChild(grid.Entity);
            this.EntityManager.Add(entity);

            var delay = TimeSpan.FromSeconds(1);
            var audioService = WaveServices.GetService<AudioService>();

            this.CreateGameAction(new ActionGameAction(() =>
            {
                countDownTextBlock.Text = "3";
                audioService.Play(Audio.Sfx.Zap_wav);
            }).Delay(delay)
            .ContinueWith(new ActionGameAction(() =>
            {
                countDownTextBlock.Text = "2";
                audioService.Play(Audio.Sfx.Zap_wav);
            }).Delay(delay)
            .ContinueWith(new ActionGameAction(() =>
            {
                countDownTextBlock.Text = "1";
                audioService.Play(Audio.Sfx.Zap_wav);
            }).Delay(delay)
            .ContinueWith(new ActionGameAction(() =>
            {
                countDownTextBlock.Text = string.Empty;
                Entity player = this.CreatePlayer();
                this.StartPlayerGamePlay(player);
            }))))).Run();
        }

        public void TestHitMyself()
        {
            this.HitFoe(this.gameplayManager, this.playerID, 50);
        }

        public void TestDieMyself()
        {
            this.DestroyFoe(this.gameplayManager, this.playerID, string.Empty);
        }

        private void ConfigurePhysics()
        {
            this.PhysicsManager.Simulation2D.Gravity = Vector2.Zero;
        }

        private void Configure3DCamera()
        {
            var centerMap = this.mapLoader.CenterMap;

            var camera = this.EntityManager.FindComponentFromEntityPath<Player3DCameraBehavior>("camera3D");

            var position = centerMap.FindComponent<Transform3D>();

            camera.Reset(position.Position);

            camera.TargetTransform = centerMap.FindComponent<Transform3D>();
            camera.Speed = 5.0f;
            camera.Follow = true;
        }

        protected override void Start()
        {
            base.Start();

            var gameplayEntity = this.EntityManager.Find(GameConstants.ManagerEntityPath);
            this.gameplayManager = gameplayEntity.FindComponent<GamePlayManager>();
            this.gameplayManager.InitializeExplosion();

            this.powerUpManager = new PowerUpManager(this.peerManager, this.mapLoader);
            gameplayEntity.AddComponent(this.powerUpManager);

            this.Configure3DCamera();
            this.powerUpManager.InitPowerUp();
            this.ConfigurePhysics();
            this.CreateCountDown();
        }

        public Entity CreatePlayer()
        {
            Entity player = this.CreatePlayer(gameplayManager);

            return player;
        }

        protected override async void End()
        {
            if (!string.IsNullOrEmpty(this.playerID))
            {
                var destroyMessage = new DestroyPlayerMessage() { PlayerId = this.playerID };
                await peerManager.SendBroadcastAsync(peerManager.CreateMessage(P2PMessageType.DestroyPlayer, destroyMessage));
            }

            base.End();
        }

        private void StartPlayerGamePlay(Entity player)
        {
            /// Create Local Player
            //Entity player = this.CreatePlayer(gameplayManager);
            this.HandlePlayerCollision(player);

            this.StartPlayerCamera(player);
        }

        private void StartPlayerCamera(Entity player)
        {
            var camera = this.RenderManager.ActiveCamera3D.Owner.FindComponent<Player3DCameraBehavior>();
            camera.TargetTransform = player.FindChild("model3D").FindComponent<Transform3D>();
            camera.Speed = 5.0f;
            camera.Follow = true;
        }

        private void HandlePlayerCollision(Entity player)
        {
            if (player == null)
                return;

            var colliders = player.FindComponentsInChildren<Collider2D>(false);
            var collider = colliders.FirstOrDefault();

            if (collider != null)
            {
                collider.BeginCollision += (contact) =>
                {
                    // Cat5 is Foe Bullet
                    if (contact.ColliderB.CollisionCategories == ColliderCategory2D.Cat5)
                    {
                        var bulletCollider = contact.ColliderB.UserData as Collider2D;
                        if (bulletCollider != null)
                        {
                            var component = bulletCollider.Owner.FindComponent<BulletComponent>();
                            var killer = component.PlayerOwnerId;
                            player.FindComponent<PlayerInputBehavior>().Hit(50, killer);
                            var bullet = bulletCollider.Owner;
                            this.gameplayManager.DestroyBullet(bullet, this.peerManager);
                        }
                    }
                    // Cat6 is Power Up
                    if (contact.ColliderB.CollisionCategories == ColliderCategory2D.Cat6)
                    {
                        var powerUpCollider = contact.ColliderB.UserData as Collider2D;
                        if (powerUpCollider != null)
                        {
                            var powerUp = powerUpCollider.Owner;
                            this.powerUpManager.SendDestroyPowerUpMessage(powerUp.Name);

                            var powerUpBehavior = powerUp.FindComponent<PowerUpBehavior>();
                            this.gameplayManager.AddPowerUp(player.Name, powerUpBehavior.PowerUpType, this.peerManager);

                            var audioService = WaveServices.GetService<AudioService>();
                            audioService.Play(Audio.Sfx.PowerUp_wav);
                        }
                    }
                };
            }
        }

        private Entity CreatePlayer(GamePlayManager gameplayManager)
        {
            // New player identifier
            // this.playerID = Guid.NewGuid().ToString();

            // Get a random spawn point to initialize the player
            var spawnIndex = WaveServices.Random.Next(0, 4);
            var spawnPoint = this.mapLoader.GetSpawnPoint(spawnIndex);

            // Create player
            var player = gameplayManager.CreatePlayer(0, peerManager, this.playerID, spawnPoint);

            var playerColor = player.FindComponent<TankComponent>().Color;

            //this.SendCreatePlayerMessage(playerColor, spawnPoint);

            return player;
        }

        private void CreateFoe(GamePlayManager gameplayManager, Color foeColor, Vector2 foeSpawnPosition, string foeID)
        {
            gameplayManager.CreateFoe(1, peerManager, foeID, foeColor, foeSpawnPosition);
            //this.SendCreatePlayerMessage(foeColor, foeSpawnPosition);
        }

        private void CreatePowerUp(GamePlayManager gameplayManager, string powerUpId, PowerUpType powerUpType, Vector2 powerUpSpawnPosition)
        {
            gameplayManager.CreatePowerUp(powerUpId, powerUpType, powerUpSpawnPosition);
        }

        private void DestroyPowerUp(GamePlayManager gameplayManager, string powerUpId)
        {
            var powerUp = this.EntityManager.Find(powerUpId);
            this.gameplayManager.DestroyPowerUp(powerUp);
        }

        private void RemovePowerUp(GamePlayManager gameplayManager)
        {
            this.gameplayManager.RemovePowerUp();
        }

        private void HitFoe(GamePlayManager gameplayManager, string foeId, double life)
        {
            var foe = this.EntityManager.Find(foeId);
            this.gameplayManager.SmokeTank(foe, life <= 50);
        }

        private void DestroyFoe(GamePlayManager gameplayManager, string foeId, string killerId)
        {
            if (!string.IsNullOrEmpty(foeId))
            {
                var foe = this.EntityManager.Find(foeId);
                this.gameplayManager.DestroyTank(foe, killerId);
            }
        }

        private void OnMsgReceived(object sender, MsgReceivedEventArgs e)
        {
            var messageReceived = Encoding.ASCII.GetString(e.Message);

            Labels.Add("MessageOnGamePlayScene", messageReceived);

            var result = peerManager.ReadMessage(messageReceived);

            if (result.Any())
            {
                var message = result.FirstOrDefault();

                if (message.Value != null)
                {
                    switch (message.Key)
                    {
                        case P2PMessageType.CreatePlayer:
                            var createPlayerData = message.Value as CreatePlayerMessage;

                            if (createPlayerData.PlayerId.Equals(this.playerID))
                            {
                                break;
                            }

                            if (!this.EntityManager.AllEntities.Any(i => i.Name.Equals(createPlayerData.PlayerId)))
                            {
                                this.CreateFoe(this.gameplayManager, createPlayerData.PlayerColor, createPlayerData.SpawnPosition, createPlayerData.PlayerId);
                            }
                            break;
                        case P2PMessageType.Move:
                            var moveData = message.Value as MoveMessage;
                            this.gameplayManager.Move(moveData.PlayerId, moveData.X, moveData.Y);
                            break;
                        case P2PMessageType.Rotate:
                            var rotateData = message.Value as RotateMessage;
                            this.gameplayManager.Rotate(rotateData.PlayerId, rotateData.Rotation);
                            break;
                        case P2PMessageType.Shoot:
                            break;
                        case P2PMessageType.HitPlayer:
                            var hitPlayerData = message.Value as HitPlayerMessage;
                            this.HitFoe(this.gameplayManager, hitPlayerData.PlayerId, hitPlayerData.PlayerLife);
                            break;
                        case P2PMessageType.DestroyPlayer:
                            var destroyPlayerData = message.Value as DestroyPlayerMessage;

                            if (destroyPlayerData.PlayerId.Equals(this.playerID))
                            {
                                break;
                            }

                            this.DestroyFoe(this.gameplayManager, destroyPlayerData.PlayerId, destroyPlayerData.KillerId);

                            break;
                        case P2PMessageType.BulletCreate:
                            var createBulletData = message.Value as BulletCreateMessage;

                            if (activeBullets.Any(b => b.Equals(createBulletData.BulletID)))
                            {
                                break;
                            }

                            this.AddActiveBullet(createBulletData.BulletID);
                            this.gameplayManager.CreateFoeBullet(createBulletData.Color, createBulletData.PlayerID, createBulletData.BulletID, peerManager);
                            break;
                        case P2PMessageType.BulletDestroy:
                            var destroyBulletData = message.Value as BulletDestroyMessage;
                            this.activeBullets.Remove(destroyBulletData.BulletId);
                            var bullet = this.EntityManager.Find(destroyBulletData.BulletId);
                            this.gameplayManager.DestroyBullet(bullet, null);
                            break;
                        case P2PMessageType.CreatePowerUp:
                            var createPowerUpMessage = message.Value as CreatePowerUpMessage;
                            this.CreatePowerUp(this.gameplayManager, createPowerUpMessage.PowerUpId, createPowerUpMessage.PowerUpType, createPowerUpMessage.SpawnPosition);
                            break;
                        case P2PMessageType.DestroyPowerUp:
                            var destroyPowerUpMessage = message.Value as DestroyPowerUpMessage;
                            this.DestroyPowerUp(this.gameplayManager, destroyPowerUpMessage.PowerUpId);
                            break;
                        case P2PMessageType.RemovePowerUp:
                            var removePowerUpMessage = message.Value as RemovePowerUpMessage;
                            this.RemovePowerUp(this.gameplayManager);
                            break;
                        case P2PMessageType.PlayerRequest:
                            var playerRequestMessage = message.Value as PlayerRequestMessage;

                            if (playerRequestMessage != null)
                            {
                                var player = this.EntityManager.Find(this.playerID);

                                if (player != null)
                                {
                                    var playerColor = player.FindComponent<TankComponent>().Color;
                                    var position = player.FindComponent<Transform2D>().Position;
                                    this.SendCreatePlayerMessage(playerColor, position);
                                }
                            }
                            break;
                    }
                }
            }
        }

        public void AddActiveBullet(string id)
        {
            this.activeBullets.Add(id);
        }

        private void OnPeerChanged(object sender, PeerPlayerChangeEventArgs e)
        {
            var localIpAddress = this.peerManager.IpAddress;

            if (localIpAddress != null)
            {
                foreach (PeerPlayer peer in e.Peers)
                {
                    if (!this.ConnectedPeers.Contains(peer))
                    {
                        this.ConnectedPeers.Add(peer);

                        if (localIpAddress != peer.IpAddress)
                        {
                            this.SendPlayerInfoRequest();

                            //var index = WaveServices.Random.Next(0, GameConstants.Palette.Count());
                            //var color = GameConstants.Palette[index];
                            //this.SendCreatePlayerMessage(color, -Vector2.One * 100, peer.IpAddress);
                        }
                    }
                }
            }
        }


        private async void SendPlayerInfoRequest()
        {
            var message = peerManager.CreateMessage(P2PMessageType.PlayerRequest, new PlayerRequestMessage());
            await peerManager.SendBroadcastAsync(message);
        }

        private async void SendCreatePlayerMessage(Color? playerColor, Vector2? spawnPosition, string ipAddress = "")
        {
            if (string.IsNullOrEmpty(this.playerID))
                return;

            var createPlayerMessage = new CreatePlayerMessage
            {
                IpAddress = ipAddress,
                PlayerId = this.playerID
            };

            if (playerColor.HasValue && spawnPosition.HasValue)
            {
                createPlayerMessage.PlayerColor = playerColor.Value;
                createPlayerMessage.SpawnPosition = spawnPosition.Value;
            }

            var message = peerManager.CreateMessage(P2PMessageType.CreatePlayer, createPlayerMessage);

            if (string.IsNullOrEmpty(ipAddress))
            {
                await peerManager.SendBroadcastAsync(message);
            }
            else
            {
                await peerManager.SendMessage(ipAddress, message, TransportType.UDP);
            }
        }
    }
}