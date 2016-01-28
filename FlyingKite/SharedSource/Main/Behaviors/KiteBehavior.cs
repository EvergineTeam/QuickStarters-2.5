#region File Description
//-----------------------------------------------------------------------------
// Flying Kite
//
// Quickstarter for Wave University Tour 2014.
// Author: Wave Engine Team
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using FlyingKite.Managers;
using FlyingKite.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WaveEngine.Common.Math;
using WaveEngine.Components.Animation;
using WaveEngine.Framework;
using WaveEngine.Framework.Diagnostic;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Physics2D;
using WaveEngine.Framework.Services;
using System.Runtime.Serialization;
using FlyingKite.Components;
using FlyingKite.Enums;
using WaveEngine.Framework.Managers;
#endregion

namespace FlyingKite.Behaviors
{
    [DataContract]
    public class KiteBehavior : Behavior
    {
        private const float GRAVITY = 2500f;
        private const float TAPPOWER = -700f;
        private const float INTRO_AMPLITUDE = 50f;

        private float speed;
        private float topMargin;
        private bool tapPressed;

        private SoundManager soundManager;

        private VirtualScreenManager virtualScreenManager;

        [RequiredComponent]
        private Transform2D transform2D = null;

        [RequiredComponent]
        private Animation2D animation2D = null;

        [RequiredComponent(isExactType: false)]
        private Collider2D collider2D = null;

        public KiteStates KiteState { get; private set; }

        public event EventHandler<KiteStates> OnStateChanged;

        public KiteBehavior()
            : base("KiteBehavior")
        {
            this.speed = 0;
        }

        protected override void Initialize()
        {
            base.Initialize();
            this.soundManager = WaveServices.GetService<SoundManager>();

            this.virtualScreenManager = this.Owner.Scene.VirtualScreenManager;

            this.topMargin = this.transform2D.Rectangle.Height / 2;

            this.SetState(KiteStates.TakeOff);

            this.SetNewColor();
        }

        protected override void Update(TimeSpan gameTime)
        {
            if (this.KiteState == KiteStates.TakeOff)
            {
                var lastSin = Math.Sin(this.speed) * INTRO_AMPLITUDE;
                this.speed += 0.05f;
                var currentSin = Math.Sin(this.speed) * INTRO_AMPLITUDE;

                this.transform2D.Y += (float)(currentSin - lastSin);
                this.transform2D.Rotation = MathHelper.ToRadians((float)Math.Sin(this.speed + 20) * 10);
            }
            else
            {
                if (this.KiteState == KiteStates.Gameplay)
                {
                    // Add the tap power if user presses the screen.
                    if (WaveServices.Input.TouchPanelState.Count() > 0
                     || WaveServices.Input.KeyboardState.IsKeyPressed(WaveEngine.Common.Input.Keys.Space))
                    {
                        if (!this.tapPressed)
                        {
                            this.speed = TAPPOWER;
                            this.tapPressed = true;
                            this.animation2D.PlayAnimation(this.animation2D.CurrentAnimation);

                            var soundToPlay = WaveServices.Random.NextBool() ? SoundManager.SOUNDS.Chop1 : SoundManager.SOUNDS.Chop2;

                            if (this.soundManager != null)
                            {
                                this.soundManager.PlaySound(soundToPlay);
                            }
                        }
                    }
                    else
                    {
                        this.tapPressed = false;
                    }

                    if (this.transform2D.Y > this.virtualScreenManager.BottomEdge)
                    {
                        this.SetState(KiteStates.Crash);
                    }
                    else
                    {
                        this.CheckForCollisions();
                    }
                }
                else if (this.KiteState == KiteStates.Crash)
                {
                    if (this.transform2D.Y > this.virtualScreenManager.BottomEdge)
                    {
                        this.SetState(KiteStates.GameOver);
                    }
                }
                else if (this.KiteState == KiteStates.CaptureStar)
                {
                    this.SetState(KiteStates.Gameplay);
                }

                // Adds the gravity
                this.speed += GRAVITY * (float)gameTime.TotalSeconds;

                // Adds the speed to the owner entity
                this.transform2D.Rotation = this.speed * 0.00060f;

                //Limit rotation to 90º
                this.transform2D.Rotation = Math.Min(this.transform2D.Rotation, MathHelper.ToRadians(90));

                this.transform2D.Y += (float)(this.speed * gameTime.TotalSeconds);

                if (this.transform2D.Y < this.topMargin)
                {
                    this.transform2D.Y = this.topMargin;
                }
            }
        }

        private void CheckForCollisions()
        {
            // Check for collisions
            foreach (Entity obstacle in this.Owner.Scene.EntityManager.FindAllByTag("OBSTACLE"))
            {
                var obstaclePairComponent = obstacle.FindComponent<ObstaclePairComponent>();

                var kiteCollisionType = obstaclePairComponent.CheckKiteCollision(this.collider2D);

                if (kiteCollisionType == KiteCollisionTypes.Obstacle)
                {
                    this.SetState(KiteStates.Crash);
                }
                else if (kiteCollisionType == KiteCollisionTypes.Star)
                {
                    obstaclePairComponent.StarAvaible = false;
                    obstaclePairComponent.ShotStarParticles();

                    this.SetState(KiteStates.CaptureStar);
                }
            }
        }

        /// <summary>
        /// Sets a kite new color.
        /// </summary>
        private void SetNewColor()
        {
            var animationNames = this.animation2D.AnimationNames;

            this.animation2D.CurrentAnimation = animationNames.ElementAt(WaveServices.Random.Next(0, animationNames.Count()));
        }

        public void SetState(KiteStates state)
        {
            var previousState = this.KiteState;

            switch (state)
            {
                case KiteStates.TakeOff:
                    this.speed = 0;
                    this.transform2D.Y = 180;
                    break;

                case KiteStates.Gameplay:
                    if (previousState == KiteStates.GameOver)
                    {
                        this.SetNewColor();
                        this.transform2D.Y = 180;
                    }

                    if (previousState == KiteStates.TakeOff
                     || previousState == KiteStates.GameOver)
                    {
                        this.speed = TAPPOWER;
                    }

                    break;

                default:
                    break;
            }

            this.KiteState = state;

            if (this.OnStateChanged != null)
            {
                this.OnStateChanged(this, this.KiteState);
            }
        }
    }
}
