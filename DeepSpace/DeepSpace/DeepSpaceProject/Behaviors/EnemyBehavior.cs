#region Using Statements
using DeepSpaceProject.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Components.Animation;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
#endregion

namespace DeepSpaceProject.Behaviors
{
    public class EnemyBehavior : Behavior
    {
        private static System.Random rnd = new System.Random();

        [RequiredComponent]
        private Transform2D transform;

        private TimeSpan timeRatio;
        private TimeSpan shootRatio;

        private float speed;
        private int deepPosition;
        private int difficultyLevel;

        public EnemyBehavior()
            : base()
        {
            transform = null;
            speed = 2;
            deepPosition = (int)-WaveServices.ViewportManager.VirtualHeight * 3;
            this.shootRatio = TimeSpan.FromMilliseconds(1200);
        }

        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();
        }

        protected override void Update(TimeSpan gameTime)
        {
            float time = (float)gameTime.TotalMilliseconds / 10;

            transform.Y += speed * time;

            // Shooting
            if (timeRatio > TimeSpan.Zero)
            {
                timeRatio -= gameTime;
            }
            else
            {
                timeRatio = shootRatio;

                var bulletManager = EntityManager.Find<BulletManager>("BulletManager");
                bulletManager.ShootBullet(false, transform.X, transform.Y + 153, 0f, 10f);
            }

            // Reset position
            if (transform.Y > WaveServices.ViewportManager.VirtualHeight)
            {
                this.Reset();
            }
        }

        private void Reset()
        {
            difficultyLevel++;
            transform.Y = deepPosition;
            transform.X = rnd.Next((int)WaveServices.ViewportManager.VirtualWidth);

            speed = speed + (float)rnd.NextDouble() / 2;

            if (speed > 7)
            {
                speed = rnd.Next(2, 5);
            }
        }

        public void Explode()
        {
            this.Reset();

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
