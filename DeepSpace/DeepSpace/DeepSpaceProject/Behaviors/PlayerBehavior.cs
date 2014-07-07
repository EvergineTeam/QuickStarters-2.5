#region Using Statements
using DeepSpaceProject.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Input;
using WaveEngine.Common.Math;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
#endregion

namespace DeepSpaceProject.Behaviors
{
    public class PlayerBehavior : Behavior
    {
        public static readonly int NumBullets = 20;

        public const float MAXSPEED = 0.5f;

        public static readonly Vector2 ACCELERATION = new Vector2(0.01f);

        private Joystick leftJoystick;
        private FireButton fireButton;

        [RequiredComponent]
        public Transform2D Transform;

        GameplayBehavior playBehavior;

        private Vector2 speed;
        private TimeSpan timeRatio;
        private TimeSpan shootRatio;

        public PlayerBehavior(GameplayBehavior behavior)
            : base()
        {
            this.playBehavior = behavior;
            this.speed = Vector2.Zero;
            this.shootRatio = TimeSpan.FromMilliseconds(250);
        }

        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            this.leftJoystick = this.EntityManager.Find<Joystick>("leftJoystick");
            this.fireButton = this.EntityManager.Find<FireButton>("fireButton");
        }

        protected override void Update(TimeSpan gameTime)
        {
            var keyboardState = WaveServices.Input.KeyboardState;
            float time = (float)gameTime.TotalMilliseconds;

            this.speed *= 0.85f;

            if (keyboardState.IsKeyPressed(Keys.Left))
            {
                this.speed.X -= ACCELERATION.X * time;
            }

            if (keyboardState.IsKeyPressed(Keys.Right))
            {
                this.speed.X += ACCELERATION.X * time;
            }

            if (keyboardState.IsKeyPressed(Keys.Up))
            {
                this.speed.Y -= ACCELERATION.Y * time;
            }

            if (keyboardState.IsKeyPressed(Keys.Down))
            {
                this.speed.Y += ACCELERATION.Y * time;
            }

            if (this.leftJoystick.IsActive)
            {
                this.speed += this.leftJoystick.Direction * time * 0.01f;
            }

            if (this.speed.X > MAXSPEED)
            {
                this.speed.X = MAXSPEED;
                this.Transform.Effect = SpriteEffects.FlipHorizontally;
            }
            else if (this.speed.X < -MAXSPEED)
            {
                this.speed.X = -MAXSPEED;
                this.Transform.Effect = SpriteEffects.None;
            }

            if (this.speed.Y > MAXSPEED)
            {
                this.speed.Y = MAXSPEED;
            }
            else if (this.speed.Y < -MAXSPEED)
            {
                this.speed.Y = -MAXSPEED;
            }

            // Shooting
            if (timeRatio > TimeSpan.Zero)
            {
                timeRatio -= gameTime;
            }

            this.Transform.X += this.speed.X * time;
            this.Transform.Y += this.speed.Y * time;

            this.Transform.XScale = 1 - ((float)Math.Abs(this.speed.X * 0.4f));

            var shooting = keyboardState.IsKeyPressed(Keys.Space) || this.fireButton.IsShooting;

            if (shooting &&
                timeRatio <= TimeSpan.Zero)
            {
                timeRatio = shootRatio;

                var bulletManager = EntityManager.Find<BulletManager>("BulletManager");
                bulletManager.ShootBullet(true, Transform.X, Transform.Y, 0f, -15f);
            }

            if (this.playBehavior.State != GameState.Game)
            {
                return;
            }

            // Bounds check
            if (Transform.X < 0)
            {
                Transform.X = 0;
                this.speed.X = 0;
            }

            if (Transform.Y < 0)
            {
                Transform.Y = 0;
                this.speed.Y = 0;
            }

            if (Transform.X > WaveServices.ViewportManager.VirtualWidth)
            {
                this.speed.X = 0;
                Transform.X = WaveServices.ViewportManager.VirtualWidth;
            }

            if (Transform.Y > WaveServices.ViewportManager.VirtualHeight)
            {
                this.speed.Y = 0;
                Transform.Y = WaveServices.ViewportManager.VirtualHeight;
            }
        }

        public void Explode()
        {
            this.Owner.Enabled = false;

            switch (WaveServices.Random.Next(0, 3))
            {
                case 0:
                    WaveServices.GetService<SoundManager>().PlaySound(SoundManager.SOUNDS.Explode0);
                    break;
                case 1:
                    WaveServices.GetService<SoundManager>().PlaySound(SoundManager.SOUNDS.Explode1);
                    break;
                case 2:
                    WaveServices.GetService<SoundManager>().PlaySound(SoundManager.SOUNDS.Explode2);
                    break;
            }
        }
    }
}
