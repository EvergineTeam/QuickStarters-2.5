using System;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;

namespace DeepSpace.Components.Enemies
{
    public class BulletBehavior : Behavior
    {
        public float SpeedX;
        public float SpeedY;

        [RequiredComponent]
        protected Transform2D transform;

        public BulletBehavior()
            : base()
        {
            this.SpeedY = 15;
        }

        protected override void Update(TimeSpan gameTime)
        {
            float time = (float)gameTime.TotalMilliseconds / 10f;

            transform.Y += SpeedY * time;
            transform.X += SpeedX * time;

            int limit = 10;
            if (transform.Y < -limit ||
                transform.X < -limit ||
                transform.Y > WaveServices.ViewportManager.VirtualHeight + limit ||
                transform.X > WaveServices.ViewportManager.VirtualWidth + limit)
            {
                Owner.Enabled = false;
            }
        }
    }
}
