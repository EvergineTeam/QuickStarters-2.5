using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P2PNET.TransportLayer;
using P2PNET.TransportLayer.EventArgs;
using P2PTank.Behaviors;
using P2PTank.Behaviors.Cameras;
using P2PTank.Components;
using P2PTank.Entities.P2PMessages;
using P2PTank.Managers;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Common.Physics2D;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Framework;
using WaveEngine.Framework.Diagnostic;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Physics2D;
using WaveEngine.TiledMap;

namespace P2PTank.Scenes
{
    public class TestScene : Scene
    {
        private List<string> activeBullets = new List<string>();

        private List<Peer> ConnectedPeers { get; set; } = new List<Peer>();


        private string contentPath;
        private P2PManager peerManager;
        private GamePlayManager gameplayManager;

        private string playerID;

        public TestScene(string contentPath)
        {
            this.contentPath = contentPath;

            this.peerManager = new P2PManager();
            this.peerManager.PeerChange += this.OnPeerChanged;
            this.peerManager.MsgReceived += this.OnMsgReceived;
        }

        protected override async void CreateScene()
        {
            this.Load(this.contentPath);

#if DEBUG
            var debugEntity = new Entity()
                .AddComponent(new DebugBehavior());
            this.EntityManager.Add(debugEntity);
#endif

            await peerManager.StartAsync();
        }

        private void ConfigurePhysics()
        {
            this.PhysicsManager.Simulation2D.Gravity = Vector2.Zero;
        }

        private void CreateBorders(Entity tiledEntity, ColliderCategory2D category, ColliderCategory2D collidesWith)
        {
            var tiledMap = tiledEntity.FindComponent<TiledMap>();
            var borders = tiledMap.ObjectLayers[GameConstants.TiledMapBordersLayerName];
            foreach (var border in borders.Objects)
            {
                var colliderEntity = TiledMapUtils.CollisionEntityFromObject(border.Name, border);
                colliderEntity.Tag = GameConstants.TagCollider;
                colliderEntity.AddComponent(new RigidBody2D() { PhysicBodyType = RigidBodyType2D.Static });

                var collider = colliderEntity.FindComponent<Collider2D>(false);
                if (collider != null)
                {
                    collider.CollisionCategories = category;
                    collider.CollidesWith = collidesWith;
                    collider.Friction = 1.0f;
                    collider.Restitution = 0.2f;
                }

                tiledEntity.AddChild(colliderEntity);
            }
        }

        protected override void Start()
        {
            base.Start();

            this.gameplayManager = this.EntityManager.FindComponentFromEntityPath<GamePlayManager>(GameConstants.ManagerEntityPath);

            ///// Doing this code here cause in CreateScene doesnt load tiledMap file still

            var tiledEntity = this.EntityManager.Find(GameConstants.MapEntityPath);

            this.ConfigurePhysics();
            this.CreateBorders(tiledEntity, ColliderCategory2D.Cat3, ColliderCategory2D.All);
            /////

            /// Create Player
            Entity player = this.CreatePlayer(gameplayManager);

            //var foe1 = gameplayManager.CreateFoe(1);
            //var foe2 = gameplayManager.CreateFoe(2);
            //var foe3 = gameplayManager.CreateFoe(3);
            //foe1.FindComponent<Transform2D>().LocalPosition = this.GetSpawnPoint(1);
            //foe2.FindComponent<Transform2D>().LocalPosition = this.GetSpawnPoint(2);
            //foe3.FindComponent<Transform2D>().LocalPosition = this.GetSpawnPoint(3);
            //this.EntityManager.Add(foe1);
            //this.EntityManager.Add(foe2);
            //this.EntityManager.Add(foe3);

            /// Set camera to follow player
            var targetCameraBehavior = new TargetCameraBehavior();
            targetCameraBehavior.SetTarget(player.FindComponent<Transform2D>());
            targetCameraBehavior.Follow = true;
            targetCameraBehavior.Speed = 5;

            var tiledMapEntity = this.EntityManager.Find(GameConstants.MapEntityPath);
            var tiledMap = tiledMapEntity.FindComponent<TiledMap>();
            var tiledMapTransform = tiledMapEntity.FindComponent<Transform2D>();

            targetCameraBehavior.SetLimits(
                new Vector2(0, 0),
                new Vector2(tiledMap.Width * tiledMap.TileWidth * tiledMapTransform.Scale.X, tiledMap.Height * tiledMap.TileHeight * tiledMapTransform.Scale.Y));
            this.RenderManager.ActiveCamera2D.Owner.AddComponent(targetCameraBehavior);
            targetCameraBehavior.RefreshCameraLimits();
        }

        private Entity CreatePlayer(GamePlayManager gameplayManager)
        {
            this.playerID = Guid.NewGuid().ToString();

            var player = gameplayManager.CreatePlayer(0, peerManager, this.playerID);
            player.FindComponent<Transform2D>().LocalPosition = this.GetSpawnPoint(0);
            this.EntityManager.Add(player);

            this.SendCreatePlayerMessage();
            return player;
        }

        private Entity CreateFoe(GamePlayManager gameplayManager, string foeID)
        {
            var foe = gameplayManager.CreateFoe(1, peerManager, foeID);
            foe.FindComponent<Transform2D>().LocalPosition = this.GetSpawnPoint(1);
            this.EntityManager.Add(foe);
            return foe;
        }

        private Vector2 GetSpawnPoint(int index)
        {
            Vector2 res = Vector2.Zero;
            var entity = this.EntityManager.Find(string.Format(GameConstants.SpawnPointPathFormat, index));

            if (entity != null)
            {
                res = entity.FindComponent<Transform2D>().LocalPosition;
            }

            return res;
        }

        private void OnMsgReceived(object sender, MsgReceivedEventArgs e)
        {
            var messageReceived = Encoding.ASCII.GetString(e.Message);
            Labels.Add("OnMsgReceived", messageReceived);

            if (messageReceived.Contains("Create"))
            {
            }

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
                                this.CreateFoe(this.gameplayManager, createPlayerData.PlayerId);
                            }

                            break;
                        case P2PMessageType.Move:
                            break;
                        case P2PMessageType.Rotate:
                            break;
                        case P2PMessageType.Shoot:
                            break;
                        case P2PMessageType.Destroy:
                            break;
                        case P2PMessageType.BulletCreate:
                            var createBulletData = message.Value as BulletCreateMessage;

                            if (activeBullets.Any(b => b.Equals(createBulletData.BulletID)))
                            {
                                break;
                            }

                            this.AddActiveBullet(createBulletData.BulletID);
                            this.gameplayManager.CreateFoeBullet(createBulletData.Color, this.playerID, createBulletData.BulletID, peerManager);
                            break;
                        case P2PMessageType.BulletDestroy:
                            var destroyBulletData = message.Value as BulletDestroyMessage;
                            this.activeBullets.Remove(destroyBulletData.BulletId);
                            break;
                    }
                }
            }
        }

        public void AddActiveBullet(string id)
        {
            this.activeBullets.Add(id);
        }

        private void OnPeerChanged(object sender, PeerChangeEventArgs e)
        {
            foreach (Peer peer in e.Peers)
            {
                Labels.Add("OnPeerChanged", peer.IpAddress);
                if (!this.ConnectedPeers.Contains(peer))
                {
                    this.ConnectedPeers.Add(peer);
                    this.SendCreatePlayerMessage(peer);
                }
            }
        }

        private async void SendCreatePlayerMessage(Peer peer = null)
        {
            var createPlayerMessage = new CreatePlayerMessage
            {
                IpAddress = string.Empty, // do we need send the IpAddress?? maybe not
                PlayerId = this.playerID,
            };

            var message = peerManager.CreateMessage(P2PMessageType.CreatePlayer, createPlayerMessage);

            if (peer == null)
            {
                await peerManager.SendBroadcastAsync(message);
            }
            else
            {
                await peerManager.SendMessage(peer.IpAddress, message, TransportType.UDP);
            }
        }
    }
}
