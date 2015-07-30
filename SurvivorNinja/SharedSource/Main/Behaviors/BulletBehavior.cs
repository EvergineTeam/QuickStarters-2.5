#region Using Statements
using SurvivorNinja.Entities;
using SurvivorNinja.Managers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Physics2D;
using WaveEngine.Framework.Services;
#endregion

namespace SurvivorNinja.Behaviors
{
    public class BulletBehavior : Behavior
    {
        [RequiredComponent]
        private Transform2D transform;

        [RequiredComponent(false)]
        private Collider2D collider;

        private float velocity = 9f;
        private Vector2 direction;
        private float rotationVelocity = MathHelper.ToRadians(15);
        private Bullet bullet;
        private HubPanel hubPanel;

        /// <summary>
        /// Initializes a new instance of the <see cref="BulletBehavior" /> class.
        /// </summary>
        /// <param name="bullet">The bullet.</param>
        public BulletBehavior(Bullet bullet)
        {
            this.bullet = bullet;
        }

        /// <summary>
        /// Resolves the dependencies needed for this instance to work.
        /// </summary>
        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            this.hubPanel = EntityManager.Find<HubPanel>("HubPanel");
        }

        /// <summary>
        /// Allows this instance to execute custom logic during its <c>Update</c>.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        /// <remarks>
        /// This method will not be executed if the <see cref="T:WaveEngine.Framework.Component" />, or the <see cref="T:WaveEngine.Framework.Entity" />
        /// owning it are not <c>Active</c>.
        /// </remarks>
        protected override void Update(TimeSpan gameTime)
        {
            if (!this.Owner.IsVisible)
                return;

            this.direction = bullet.Direction;
            //Debug.WriteLine("direction: " + this.direction);
            float fps = 60 * (float)gameTime.TotalSeconds;
            this.transform.X += this.direction.X * velocity * fps;
            this.transform.Y += this.direction.Y * velocity * fps;
            this.transform.Rotation += rotationVelocity * fps;

            // Limits
            if (this.transform.X < WaveServices.ViewportManager.LeftEdge || this.transform.X > WaveServices.ViewportManager.RightEdge ||
                this.transform.Y < WaveServices.ViewportManager.TopEdge || this.transform.Y > WaveServices.ViewportManager.BottomEdge)
            {
                this.Owner.IsVisible = false;
            }

            // Enemies            
            foreach (Entity enemy in EntityManager.FindAllByTag("enemy"))
            {
                if (enemy.IsVisible)
                {
                    Collider2D enemyCollider = enemy.FindComponent<Collider2D>(false);
                    if (collider.Intersects(enemyCollider))
                    {
                        // Kill enemy
                        enemy.IsVisible = false;
                        this.hubPanel.Murders++;

                        this.Owner.IsVisible = false;

                        SoundsManager.Instance.PlaySound(SoundsManager.SOUNDS.EnemyDead);
                        SoundsManager.Instance.PlaySound(SoundsManager.SOUNDS.Impact);
                        break;
                    }
                }
            }
        }
    }
}
