using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using P2PTank.Entities.P2PMessages;
using P2PTank.Managers;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Physics2D;

namespace P2PTank.Behaviors
{
    public class BulletNetworkBehavior : Behavior
    {
        [RequiredComponent]
        private Transform2D transform = null;

        [RequiredComponent]
        private RigidBody2D rigidBody = null;

        public string BulletID { get; set; }

        public string PlayerID { get; set; }

        private P2PManager peerManager;

        private GamePlayManager gamePlayManager;

        public BulletNetworkBehavior(P2PManager p2pManager, string bulletID, string playerID)
        {
            this.peerManager = p2pManager;
            this.BulletID = bulletID;
            this.PlayerID = playerID;

            this.peerManager.MsgReceived += this.OnMessageReceived;
        }

        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            this.gamePlayManager = this.Owner.Scene.EntityManager.FindComponentFromEntityPath<GamePlayManager>(GameConstants.ManagerEntityPath);
        }

        protected override void Update(TimeSpan gameTime)
        {
        }

        private void OnMessageReceived(object sender, P2PNET.TransportLayer.EventArgs.MsgReceivedEventArgs e)
        {
            var messageReceived = Encoding.ASCII.GetString(e.Message);

            var result = this.peerManager.ReadMessage(messageReceived);

            if (result.Any())
            {
                var message = result.FirstOrDefault();

                if (message.Value != null)
                {
                    switch (message.Key)
                    {
                        case P2PMessageType.BulletMove:
                            var moveData = message.Value as BulletMoveMessage;

                            if (this.BulletID.Equals(moveData.BulletId))
                            {
                                this.BulletMove(moveData.X, moveData.Y);
                            }
                            break;
                        case P2PMessageType.BulletDestroy:
                            var destroyData = message.Value as BulletDestroyMessage;

                            if (this.BulletID.Equals(destroyData.BulletId))
                            {
                                this.BulletDestroy();
                            }
                            break;
                    }
                }
            }
        }

        private void BulletDestroy()
        {
            this.gamePlayManager.DestroyBullet(this.Owner, this.peerManager);
        }

        private void BulletMove(float x, float y)
        {
            if (this.rigidBody != null && this.rigidBody.RigidBody != null)
            {
                this.rigidBody.ResetPosition(new Vector2(x, y));
            }
        }
    }
}
