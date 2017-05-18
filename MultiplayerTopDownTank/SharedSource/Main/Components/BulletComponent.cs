using System.Runtime.Serialization;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Physics2D;

namespace MultiplayerTopDownTank.Components
{
    [DataContract]
    public class BulletComponent : Component
    {
        private float velocity = 30f;
        [RequiredComponent]
        private RigidBody2D rigidBody = null;

        public void Shoot(Vector2 position, Vector2 direction)
        {
            this.rigidBody.ResetPosition(position);

            Vector2 impulse = (direction * this.velocity) / 60;
            this.rigidBody.ApplyLinearImpulse(impulse, position);
        }
    }
}