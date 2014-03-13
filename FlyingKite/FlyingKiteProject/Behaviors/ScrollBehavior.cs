using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;

namespace FlyingKiteProject.Behaviors
{
    public class ScrollBehavior : Behavior
    {
        private const float SCROLL_VELOCITY = 230;

        [RequiredComponent]
        private Transform2D transform2D;

        private float parallaxFactor;

        public Action<Entity> EntityOutOfScreen;

        public ScrollBehavior(float parallaxFactor)
        {
            this.parallaxFactor = parallaxFactor;
        }

        protected override void Update(TimeSpan gameTime)
        {
            this.transform2D.X -= SCROLL_VELOCITY * this.parallaxFactor * (float)gameTime.TotalSeconds;

            var trueWidth = this.transform2D.Rectangle.Width * this.transform2D.XScale;

            var leftSize = this.transform2D.X + trueWidth;

            if (leftSize < WaveServices.ViewportManager.LeftEdge)
            {
                this.OnEntityOutOfScreen(this.Owner);
            }
        }

        private void OnEntityOutOfScreen(Entity entity)
        {
            if (this.EntityOutOfScreen != null)
                this.EntityOutOfScreen(entity);
        }
    }
}
