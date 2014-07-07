#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Components.Graphics2D;
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

        [RequiredComponent]
        private Sprite sprite;

        private float speed;

        private float margin;

        public StarBehavior(float speed, float margin)
            : base()
        {
            this.transform = null;
            this.speed = speed;
            this.margin = margin;
        }

        protected override void Update(TimeSpan gameTime)
        {
            float time = (float)gameTime.TotalMilliseconds / 10f;

            var displace = speed * time;
            transform.Y += displace;

            transform.YScale = ((displace * 0.5f) + this.sprite.Texture.Height) / this.sprite.Texture.Height;

            if (transform.Y > WaveServices.ViewportManager.BottomEdge + margin)
            {
                transform.Y = WaveServices.ViewportManager.TopEdge - margin;
                transform.X = (float)WaveServices.Random.NextDouble() * WaveServices.ViewportManager.VirtualWidth;
            }
        }
    }
}
