#region Using Statements
using FlyingKite.Managers;
using FlyingKiteProject.Entities;
using FlyingKiteProject.Scenes;
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
#endregion

namespace FlyingKiteProject.Behaviors
{
    public class KiteBehavior : Behavior
    {
        public enum KiteStates
        {
            TakeOff,
            Gameplay,
            Gameover
        };

        private const float GRAVITY = 2500f;
        private const float TAPPOWER = -700f;
        private const float INTRO_AMPLITUDE = 50f;

        private float speed;
        private float topMargin;
        private bool tapPressed;

        private GameScene scene;

        private SoundManager soundManager;

        [RequiredComponent]
        private Transform2D transform2D;

        [RequiredComponent]
        private Animation2D animation2D;

        [RequiredComponent(isExactType: false)]
        private Collider2D collider2D;

        public KiteStates KiteState {get; private set;}

        public KiteBehavior()
            : base("KiteBehavior")
        {
            this.speed = 0;
        }

        protected override void Initialize()
        {
            base.Initialize();
            this.scene = (GameScene)this.Owner.Scene;
            this.soundManager = WaveServices.GetService<SoundManager>();

            this.topMargin = this.transform2D.Rectangle.Height / 2;

            this.SetState(KiteStates.TakeOff);
        }

        protected override void Update(TimeSpan gameTime)
        {
            if (this.KiteState == KiteStates.TakeOff)
            {
                var lastSin = Math.Sin(this.speed) * INTRO_AMPLITUDE;
                this.speed+= 0.05f;
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
                            this.animation2D.Play();

                            var soundToPlay = WaveServices.Random.NextBool() ? SoundManager.SOUNDS.Chop1 : SoundManager.SOUNDS.Chop2;
                            this.soundManager.PlaySound(soundToPlay);
                        }
                    }
                    else
                    {
                        this.tapPressed = false;
                    }

                    if (this.transform2D.Y > WaveServices.ViewportManager.BottomEdge)
                    {
                        this.SetState(KiteStates.Gameover);
                    }
                    else
                    {
                        this.CheckForCollisions();
                    }
                }
                else if (this.KiteState == KiteStates.Gameover)
                {
                    if (this.transform2D.Y > WaveServices.ViewportManager.BottomEdge 
                        && this.scene.CurrentState != GameScene.GameSceneStates.GameOver)
                    {
                        this.scene.SetState(GameScene.GameSceneStates.GameOver);
                    }
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
                    //this.speed = 0;
                }
            }
        }

        private void CheckForCollisions()
        {
            var ownerLeftPosition = (this.transform2D.Rectangle.Width * Math.Abs(1 - this.transform2D.Origin.X)) + this.transform2D.X;

            // Check for collisions
            foreach (var obstacle in this.Owner.Scene.EntityManager.FindAllByTag("OBSTACLE"))
            {
                var obstaclePair = (ObstaclePair)obstacle;

                //Check if the kite is crossing this obstacle
                if (ownerLeftPosition > obstaclePair.Transform2D.X)
                {
                    Labels.Add("Collision", obstaclePair.Name);

                    //If collides with any of the obstacles inside the pair the games ends
                    if (this.collider2D.Intersects(obstaclePair.TopCollider)
                        || this.collider2D.Intersects(obstaclePair.BottomCollider))
                    {
                        this.SetState(KiteStates.Gameover);
                    }
                    else if (obstaclePair.StarAvaible && this.collider2D.Intersects(obstaclePair.StarCollider))
                    {
                        obstaclePair.StarAvaible = false;
                        obstaclePair.ShotStarParticles();
                        this.soundManager.PlaySound(SoundManager.SOUNDS.Coin);
                        this.scene.CurrentScore++;
                    }
                }
            }
        }

        public void SetState(KiteStates state)
        {
            switch (state)
            {
                case KiteStates.TakeOff:
                    this.speed = 0;
                    this.transform2D.Y = WaveServices.ViewportManager.VirtualHeight / 2;
                    break;
                case KiteStates.Gameplay:
                    if (this.KiteState == KiteStates.Gameover)
                    {
                        this.transform2D.Y = WaveServices.ViewportManager.VirtualHeight / 2;
                    }

                    this.speed = TAPPOWER;
                    break;
                case KiteStates.Gameover:
                    this.soundManager.PlaySound(SoundManager.SOUNDS.Crash);
                    this.scene.SetState(GameScene.GameSceneStates.Crash);
                    break;
                default:
                    break;
            }

            this.KiteState = state;
        }
    }
}
