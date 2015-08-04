using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using WaveEngine.Framework;
using WaveEngine.Framework.Physics2D;
using WaveEngine.Framework.Graphics;
using WaveEngine.Common.Attributes;

namespace SuperSquid.Components
{
    [DataContract(Namespace = "SuperSquid.Components")]
    public class RocksBlock : Component
    {
        private List<Collider2D> rocks;
        private List<Collider2D> jellyFish;
        private List<Collider2D> stars;

        [RequiredComponent]
        [DontRenderProperty]
        public Transform2D Transform2D { get; set; }

        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            this.rocks = this.Owner.FindChildrenByTag("Rock").Select( e => e.FindComponent<Collider2D>(false)).ToList();
            this.jellyFish = this.Owner.FindChildrenByTag("JellyFish").Select(e => e.FindComponent<Collider2D>(false)).ToList();
            this.stars = this.Owner.FindChildrenByTag("StarFish").Select(e => e.FindComponent<Collider2D>(false)).ToList();
        }

        /// <summary>
        /// Check if any rock element collides with the specified collider.
        /// </summary>
        /// <param name="collider">The collider.</param>
        /// <returns></returns>
        public bool CheckRockCollision(Collider2D collider)
        {
            return this.CheckCollectionCollision(collider, this.rocks) != null;
        }

        /// <summary>
        /// Check if any jellyfish element collides with the specified collider.
        /// </summary>
        /// <param name="collider">The collider.</param>
        /// <returns></returns>
        public bool CheckJellyFishCollision(Collider2D collider)
        {
            return this.CheckCollectionCollision(collider, this.jellyFish) != null;
        }

        /// <summary>
        /// Check if any star element collides with the specified collider.
        /// </summary>
        /// <param name="collider">The collider.</param>
        /// <returns></returns>
        public bool CheckStarCollision(Collider2D collider)
        {
            var collision = this.CheckCollectionCollision(collider, this.stars);

            if(collision != null)
            {
                collision.Owner.IsVisible = false;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Reset this instance
        /// </summary>
        public void Reset()
        {
            foreach (Collider2D star in this.stars)
            {
                star.Owner.IsVisible = true;
            }
        }

        /// <summary>
        /// Checks if any collider in the specified collection collides with the spiecified collider.
        /// </summary>
        /// <param name="collider">The collider.</param>
        /// <param name="collection">The collider collection</param>
        /// <returns></returns>
        private Collider2D CheckCollectionCollision(Collider2D collider, List<Collider2D> collection)
        {
            Collider2D collided = null;

            foreach (Collider2D colliderB in collection)
            {
                if (colliderB.Owner.IsVisible  && collider.Intersects(colliderB))
                {
                    collided = colliderB;
                    break;
                }
            }

            return collided;
        }
    }
}
