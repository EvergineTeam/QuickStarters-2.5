#region File Description
//-----------------------------------------------------------------------------
// Flying Kite
//
// Quickstarter for Wave University Tour 2014.
// Author: Wave Engine Team
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using FlyingKite.Behaviors;
using FlyingKite.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Math;
using WaveEngine.Components.Particles;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Managers;
using WaveEngine.Framework.Physics2D;
using WaveEngine.Framework.Services;
#endregion

namespace FlyingKite.Components
{
    [DataContract]
    public class ObstaclePairComponent : Component
    {
        private const int DISTANCE_BWT_OBSTACLEPAIRS = 500;

        private const int REAPPEARANCE_X = 4 * DISTANCE_BWT_OBSTACLEPAIRS;

        [RequiredComponent]
        private Transform2D transform2D = null;

        [RequiredComponent]
        private ScrollBehavior scrollBehavior = null;

        [RequiredComponent(isExactType: false)]
        private Collider2D obstaclePairCollider = null;

        private Collider2D topCollider = null;

        private Collider2D bottomCollider = null;

        private Collider2D starCollider = null;

        private VirtualScreenManager virtualScreenManager;

        private Entity starEntity;

        private Entity starParticles;

        private float initialXPosition;

        [DontRenderProperty]
        public bool StarAvaible
        {
            get
            {
                return this.starEntity.IsVisible;
            }

            set
            {
                this.starEntity.IsVisible = value;
            }
        }

        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            this.virtualScreenManager = this.Owner.Scene.VirtualScreenManager;

            this.starEntity = this.Owner.FindChild("star");

            this.starParticles = this.Owner.FindChild("starParticles");

            this.topCollider = this.Owner.FindChild("top")
                                         .FindComponent<Collider2D>(isExactType: false);

            this.bottomCollider = this.Owner.FindChild("bottom")
                                         .FindComponent<Collider2D>(isExactType: false);

            this.starCollider = this.Owner.FindChild("star")
                                         .FindComponent<Collider2D>(isExactType: false);

            this.scrollBehavior.OnEntityOutOfScreen += (sender, entity) =>
            {
                this.transform2D.X += REAPPEARANCE_X;
                this.transform2D.Y = this.GetRandomY();
                this.StarAvaible = true;
            };
        }

        protected override void Initialize()
        {
            base.Initialize();

            if (!this.isInitialized)
            {
                this.initialXPosition = this.transform2D.X;
            }
        }

        /// <summary>
        /// Gets a new random Y position.
        /// </summary>
        /// <returns></returns>
        private float GetRandomY()
        {
            var randomOffsetY = (int)(this.transform2D.Rectangle.Height * 0.10f);
            return (this.virtualScreenManager.VirtualHeight * 0.5f) + WaveServices.Random.Next(-randomOffsetY, randomOffsetY);
        }

        /// <summary>
        /// Checks the kite collision.
        /// </summary>
        /// <param name="kiteCollider2D">The kite collider2D.</param>
        /// <returns></returns>
        public KiteCollisionTypes CheckKiteCollision(Collider2D kiteCollider2D)
        {
            var collisionResult = KiteCollisionTypes.None;

            //Check if the kite is crossing this obstacle
            if (this.obstaclePairCollider.Intersects(kiteCollider2D))
            {
                //If collides with any of the obstacles inside the pair the games ends
                if (this.topCollider.Intersects(kiteCollider2D)
                || this.bottomCollider.Intersects(kiteCollider2D))
                {
                    collisionResult = KiteCollisionTypes.Obstacle;
                }
                else if (this.StarAvaible
                    && this.starCollider.Intersects(kiteCollider2D))
                {
                    collisionResult = KiteCollisionTypes.Star;
                }
            }

            return collisionResult;
        }

        /// <summary>
        /// Shots the star particles.
        /// </summary>
        public void ShotStarParticles()
        {
            var particleSystem = this.starParticles.FindComponent<ParticleSystem2D>();

            particleSystem.Emit = true;
            WaveServices.TimerFactory.CreateTimer("explosionTimer", TimeSpan.FromSeconds(0.6f), () =>
            {
                particleSystem.Emit = false;
            }, false);
        }

        public void ResetState()
        {
            if (this.isInitialized)
            {
                this.transform2D.X = this.initialXPosition;

                this.Owner.Enabled = true;
                this.StarAvaible = true;
            }
        }
    }
}
