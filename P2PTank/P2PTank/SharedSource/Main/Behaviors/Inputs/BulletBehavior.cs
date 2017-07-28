using System;
using System.Collections.Generic;
using System.Text;
using P2PTank.Components;
using P2PTank.Managers;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
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

        private P2PManager p2pManager;

        private GamePlayManager gamePlayManger;

        public string BulletID { get; set; }

        public BulletBehavior(P2PManager p2pManager = null)
        {
            this.p2pManager = p2pManager;
        }

        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            this.collider.BeginCollision += this.ColliderBeginCollision;

            this.gamePlayManger = this.Owner.Scene.EntityManager.FindComponentFromEntityPath<GamePlayManager>(GameConstants.ManagerEntityPath);
        }

        private void ColliderBeginCollision(WaveEngine.Common.Physics2D.ICollisionInfo2D contact)
        {
       
            this.gamePlayManger.DestroyBullet(this.Owner);

            if (this.p2pManager != null)
            {
                p2pManager.CreateMessage(Entities.P2PMessages.P2PMessageType.Destroy, this.BulletID);
            }
        }

        public void Shoot(Vector2 position, Vector2 direction)
        {
            this.rigidBody.ResetPosition(position);
            var impulse = (direction * this.bulletComponent.CurrentSpeed) / 60;
            this.rigidBody.ApplyLinearImpulse(impulse, position);
        }

        protected override void Update(TimeSpan gameTime)
        {
        }
    }
}
