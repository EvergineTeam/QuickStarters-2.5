#region File Description
//-----------------------------------------------------------------------------
// Super Squid
//
// Quickstarter for Wave University Tour 2014.
// Author: Wave Engine Team
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using SuperSquidProject.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Input;
using WaveEngine.Common.Math;
using WaveEngine.Components.Animation;
using WaveEngine.Framework;
using WaveEngine.Framework.Diagnostic;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Physics2D;
using WaveEngine.Framework.Services;
#endregion

namespace SuperSquidProject.Entities.Behaviors
{
    public class SquidBehavior : Behavior, IDisposable
    {
        [RequiredComponent]
        private Transform2D transform2D;

        [RequiredComponent]
        private Animation2D animation2D;

        private BlockBuilderBehavior blockBuilderBehavior;

        private Input inputManager;
        private SoundManager soundManager;

        private float maxRotation = MathHelper.ToRadians(20);
        private float currentRotation;

        private bool tapDetected;

        /// <summary>
        /// Initializes a new instance of the <see cref="SquidBehavior" /> class.
        /// </summary>
        public SquidBehavior()
        {
            this.inputManager = WaveServices.Input;
            this.soundManager = WaveServices.GetService<SoundManager>();
        }

        /// <summary>
        /// Performs further custom initialization for this instance.
        /// </summary>
        /// <remarks>
        /// By default this method does nothing.
        /// </remarks>
        protected override void Initialize()
        {
            base.Initialize();

            // Start accelerometer if it is avaible
            if (this.inputManager.AccelerometerState.IsConnected)
            {
                this.inputManager.StartAccelerometer();
            }
        }

        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            this.blockBuilderBehavior = this.EntityManager.Find("BlockBuilder")
                                                          .FindComponent<BlockBuilderBehavior>();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            // Stop accelerometer if it is avaible
            if (this.inputManager.AccelerometerState.IsConnected)
            {
                this.inputManager.StopAccelerometer();
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
            float previousX = this.transform2D.X;

            // Horizontal displacement
            if (this.inputManager.AccelerometerState.IsConnected)
            {
                float accVal;

                // Metro devices plays in landscape orientation mode
                if (WaveServices.Platform.PlatformName == "Metro")
                {
                    accVal = -this.inputManager.AccelerometerState.SmoothAcceleration.Y * 1.5f;
                }
                else
                {
                    accVal = this.inputManager.AccelerometerState.SmoothAcceleration.X;
                }

                Labels.Add("Acc val", accVal.ToString());

                this.transform2D.X += accVal * (float)gameTime.TotalMilliseconds;
            }
            else if (this.inputManager.KeyboardState.IsKeyPressed(Keys.Right))
            {
                transform2D.X += 0.25f * (float)gameTime.TotalMilliseconds;
            }
            else if (this.inputManager.KeyboardState.IsKeyPressed(Keys.Left))
            {
                transform2D.X -= 0.25f * (float)gameTime.TotalMilliseconds;
            }

            //Horizontal displacement limit
            if (this.transform2D.X > WaveServices.ViewportManager.VirtualWidth)
            {
                this.transform2D.X = WaveServices.ViewportManager.VirtualWidth;
            }
            else if (this.transform2D.X < 0)
            {
                this.transform2D.X = 0;
            }

            // Vectical impulse by Touch tap, Up key or Space key
            if (this.inputManager.TouchPanelState.Count > 0 
            || this.inputManager.KeyboardState.IsKeyPressed(Keys.Up)
            || this.inputManager.KeyboardState.IsKeyPressed(Keys.Space))
            {
                if (!this.tapDetected)
                {
                    this.tapDetected = true;
                    this.animation2D.Play(true);
                    this.blockBuilderBehavior.ApplyImpulse();

                    //Play squid swim sound
                    var sound = WaveServices.Random.NextBool() ? SoundManager.SOUNDS.SquidSwim1 : SoundManager.SOUNDS.SquidSwim2;
                    this.soundManager.PlaySound(sound);
                }
            }
            else
            {
                this.tapDetected = false;
            }

            //Rotation
            var diffX = this.transform2D.X - previousX;
            this.currentRotation = MathHelper.Lerp(this.currentRotation, diffX * 0.1f, 0.5f);

            //Rotation limit
            if (this.currentRotation > this.maxRotation)
            {
                this.currentRotation = this.maxRotation;
            }
            else if (this.currentRotation < -this.maxRotation)
            {
                this.currentRotation = -this.maxRotation;
            }

            this.transform2D.Rotation = this.currentRotation;
        }
    }
}
