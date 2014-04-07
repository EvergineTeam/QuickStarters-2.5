#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Input;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
#endregion

namespace DeepSpaceProject.Behaviors
{
    public class PlayerBehavior : Behavior
    {
        public static readonly int NumBullets = 20;

        [RequiredComponent]
        private Transform2D transform;

        private float Speed { get; set; }
        private TimeSpan timeRatio;
        private TimeSpan shootRatio;

        public PlayerBehavior()
            : base()
        {
            this.Speed = 2f;
            this.shootRatio = TimeSpan.FromMilliseconds(250);
        }

        protected override void Update(TimeSpan gameTime)
        {
            var keyboardState = WaveServices.Input.KeyboardState;
            float time = (float)gameTime.TotalMilliseconds / 10f;

            if (keyboardState.IsKeyPressed(Keys.Left))
            {
                transform.X -= Speed * time;
            }

            if (keyboardState.IsKeyPressed(Keys.Right))
            {
                transform.X += Speed * time;
            }

            if (keyboardState.IsKeyPressed(Keys.Up))
            {
                transform.Y -= Speed * time;
            }

            if (keyboardState.IsKeyPressed(Keys.Down))
            {
                transform.Y += Speed * time;
            }

            // Shooting
            if (timeRatio > TimeSpan.Zero)
            {
                timeRatio -= gameTime;
            }

            if (keyboardState.IsKeyPressed(Keys.Space) &&
                timeRatio <= TimeSpan.Zero)
            {
                timeRatio = shootRatio;

                var BulletManager = EntityManager.Find<BulletManager>("BulletManager");
                BulletManager.ShootBullet(transform.X, transform.Y, 0f, -5f);
            }

            // Bounds check
            if (transform.X < 0)
            {
                transform.X = 0;
            }

            if (transform.Y < 0)
            {
                transform.Y = 0;
            }

            if (transform.X > WaveServices.ViewportManager.VirtualWidth)
            {
                transform.X = WaveServices.ViewportManager.VirtualWidth;
            }

            if (transform.Y > WaveServices.ViewportManager.VirtualHeight)
            {
                transform.Y = WaveServices.ViewportManager.VirtualHeight;
            }
        }
    }
}
