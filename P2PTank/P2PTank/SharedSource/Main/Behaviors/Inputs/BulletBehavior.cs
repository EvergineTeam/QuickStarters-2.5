using System;
using System.Collections.Generic;
using System.Text;
using P2PTank.Components;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Physics2D;

namespace P2PTank.Behaviors
{
    public class BulletBehavior : Behavior
    {
        [RequiredComponent]
        private RigidBody2D rigidBody = null;

        [RequiredComponent]
        private BulletComponent bulletComponent = null;

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
