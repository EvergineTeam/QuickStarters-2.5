#region Usings Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
#endregion

namespace DeepSpaceProject.Behaviors
{
    public class BulletBehavior : Behavior
    {
        public float SpeedX;
        public float SpeedY;

        [RequiredComponent]
        private Transform2D transform;

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
