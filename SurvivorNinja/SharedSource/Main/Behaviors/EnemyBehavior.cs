#region Using Statements
using SurvivorNinja.Entities;
using SurvivorNinja.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services; 
#endregion

namespace SurvivorNinja.Behaviors
{
    public class EnemyBehavior : Behavior
    {
        [RequiredComponent]
        private Transform2D transform;
        private Transform2D playerTransform;
        private PlayerBehavior player;
        private float playerRadius = 30;

        private Vector2 position, playerPosition;
        private float velocity = 4f;
        private Vector2 textureDirection = new Vector2(0, -1);        

        /// <summary>
        /// Resolves the dependencies needed for this instance to work.
        /// </summary>
        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            var playerEntity = this.EntityManager.Find("Player");

            if (playerEntity != null)
            {
                this.player = playerEntity.FindComponent<PlayerBehavior>();
                this.playerTransform = playerEntity.FindComponent<Transform2D>();
            }
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
            
            this.position.X = this.transform.X;
            this.position.Y = this.transform.Y;
            this.playerPosition.X = this.playerTransform.X;
            this.playerPosition.Y = this.playerTransform.Y;

            // Position
            Vector2 distance = this.playerPosition - this.position;
            Vector2 moveDirection = distance;
            moveDirection.Normalize();

            float fps = 60 * (float)gameTime.TotalSeconds;
            this.transform.X += moveDirection.X * this.velocity * fps;
            this.transform.Y += moveDirection.Y * this.velocity * fps;

            // Rotation
            float rotation = Vector2.Angle(moveDirection, this.textureDirection);
            this.transform.Rotation = rotation;

            if (distance.Length() < this.playerRadius)
            {
                // Player to hurt
                this.player.Life -= 50;

                this.Owner.IsVisible = false;

                SoundsManager.Instance.PlaySound(SoundsManager.SOUNDS.playerHurt);
            }
        }
    }
}
