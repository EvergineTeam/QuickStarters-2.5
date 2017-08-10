using System;
using P2PTank.Components;
using P2PTank.Entities.P2PMessages;
using P2PTank.Managers;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Physics2D;

namespace P2PTank.Behaviors
{
    public class BulletBehavior : Behavior
    {
        [RequiredComponent]
        private RigidBody2D rigidBody = null;

        [RequiredComponent(false)]
        private Collider2D collider = null;

        [RequiredComponent]
        private BulletComponent bulletComponent = null;

        [RequiredComponent]
        private Transform2D transform = null;

        private P2PManager peerManager;

        private GamePlayManager gamePlayManager;

        public string BulletID { get; set; }

        public string PlayerID { get; set; }

        public BulletBehavior(P2PManager peerManager, string bulletID, string playerID)
        {
            this.peerManager = peerManager;

            this.BulletID = bulletID;
            this.PlayerID = playerID;
        }

        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            this.collider.BeginCollision -= this.ColliderBeginCollision;
            this.collider.BeginCollision += this.ColliderBeginCollision;

            this.gamePlayManager = this.Owner.Scene.EntityManager.FindComponentFromEntityPath<GamePlayManager>(GameConstants.ManagerEntityPath);
        }

        private void ColliderBeginCollision(WaveEngine.Common.Physics2D.ICollisionInfo2D contact)
        {
            this.gamePlayManager.DestroyBullet(this.Owner, null);
        }

        public void Shoot(Vector2 position, Vector2 direction)
        {
            this.rigidBody.ResetPosition(position);
            var impulse = (direction * this.bulletComponent.CurrentSpeed) / 60;
            this.rigidBody.ApplyLinearImpulse(impulse, position);
        }

        protected override void Update(TimeSpan gameTime)
        {
            this.HandleNetworkMessages();
        }

        private async void HandleNetworkMessages()
        {
            if (this.peerManager != null)
            {
                if (this.rigidBody.Awake)
                {
                    var moveMessage = new BulletMoveMessage()
                    {
                        PlayerId = this.PlayerID,
                        BulletId = this.BulletID,
                        X = this.transform.LocalPosition.X,
                        Y = this.transform.LocalPosition.Y,
                    };

                    await this.peerManager.SendBroadcastAsync(this.peerManager.CreateMessage(P2PMessageType.BulletMove, moveMessage));
                }
            }
        }
    }
}
