#region Using Statements
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
    public class StarBehavior : Behavior
    {
        [RequiredComponent]
        private Transform2D transform;

        private float speed;

        public StarBehavior(float speed)
            : base()
        {
            this.transform = null;
            this.speed = speed;
        }

        protected override void Update(TimeSpan gameTime)
        {
            float time = (float)gameTime.TotalMilliseconds / 10f;
            transform.Y += speed * time;

            if (transform.Y > WaveServices.ViewportManager.VirtualHeight)
            {
                transform.Y = 0;
            }
        }
    }
}
