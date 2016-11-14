using System.Runtime.Serialization;
using SuperSlingshot;
using SuperSlingshot.Behaviors;
using SuperSlingshot.Enums;
using WaveEngine.Common.Physics2D;
using WaveEngine.Framework;
using WaveEngine.Framework.Physics2D;

namespace SuperSlingshot.Components
{
    [DataContract]
    public class BreakableComponent : Component
    {
        private const float PLAYERTOBREAKABLEDAMAGEMAGNITUDE = 0.5f;
        private const float BREAKABLETOBREAKABLEDAMAGEMAGNITUDE = 0.1f;

        public float currentEnergy;
        private float middleEnergy;

        [RequiredComponent]
        private PolygonCollider2D collider = null;

        [RequiredComponent]
        private BreakableBehavior behavior = null;

        [DataMember]
        public BreakableType Breakabletype { get; set; }

        [DataMember]
        public float TotalEnergy { get; set; }

        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            this.currentEnergy = this.TotalEnergy;
            this.middleEnergy = this.TotalEnergy / 2;

            this.collider.BeginCollision += this.OnBeginCollision;
            this.collider.EndCollision += this.OnEndCollision;

            this.behavior.SetState(BreakableState.NORMAL);
        }

        private void OnBeginCollision(ICollisionInfo2D contact)
        {
            // against floor
            if ((contact.ColliderB.CollisionCategories & ColliderCategory2D.Cat3) == ColliderCategory2D.Cat3)
            {
                return;
            }
            else
            {
                var relativeVelocity = contact.ColliderA.RigidBody.LinearVelocity - contact.ColliderB.RigidBody.LinearVelocity;
                var velocityMagnitude = relativeVelocity.Length();

                float damageMagnitude = 0;

                // against other breakable
                if ((contact.ColliderB.CollisionCategories & ColliderCategory2D.Cat2) == ColliderCategory2D.Cat2)
                {
                    damageMagnitude = BREAKABLETOBREAKABLEDAMAGEMAGNITUDE;
                }
                // against player
                else
                {
                    damageMagnitude = PLAYERTOBREAKABLEDAMAGEMAGNITUDE;
                }

                var totalDamage = velocityMagnitude * damageMagnitude;

                this.Hit(totalDamage);
            }
        }

        private void OnEndCollision(ICollisionInfo2D contact)
        {
        }

        public void Hit(float damage)
        {
            this.currentEnergy -= damage;

            if (this.currentEnergy <= this.middleEnergy
                && this.currentEnergy > 0)
            {
                this.behavior.SetState(BreakableState.DAMAGED);
            }
            else if (this.currentEnergy <= 0)
            {
                this.behavior.SetState(BreakableState.DEAD);
            }
        }

        protected override void DeleteDependencies()
        {
            base.DeleteDependencies();

            this.collider.BeginCollision -= this.OnBeginCollision;
            this.collider.EndCollision -= this.OnEndCollision;
        }
    }
}
