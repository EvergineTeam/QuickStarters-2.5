using System;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Managers;
using WaveEngine.Framework.Services;

namespace DeepSpace.Components.Bullets
{
    public class BulletBehavior : Behavior
    {
        public float SpeedX;
        public float SpeedY;

        [RequiredComponent]
        protected Transform2D transform;
        private VirtualScreenManager virtualScreenManager;

        public BulletBehavior()
            : base()
        {
            this.SpeedY = 15;
        }

        protected override void Initialize()
        {
            base.Initialize();

            this.virtualScreenManager = this.Owner.Scene.VirtualScreenManager;
        }

        protected override void Update(TimeSpan gameTime)
        {
            float time = (float)gameTime.TotalMilliseconds / 10f;

            this.transform.Y += SpeedY * time;
            this.transform.X += SpeedX * time;

            int limit = 10;
            if (this.transform.Y < this.virtualScreenManager.TopEdge -limit ||
                this.transform.X < this.virtualScreenManager.LeftEdge -limit ||
                this.transform.Y > this.virtualScreenManager.BottomEdge + limit ||
                this.transform.X > this.virtualScreenManager.RightEdge + limit)
            {
                this.Owner.Enabled = false;
            }
        }
    }
}
