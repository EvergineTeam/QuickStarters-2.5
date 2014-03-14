#region File Description
//-----------------------------------------------------------------------------
// OrbitRabbits
//
// Quickstarter for Wave University Tour 2014.
// Author: Wave Engine Team
//-----------------------------------------------------------------------------
#endregion

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

namespace OrbitRabbitsProject.Entities.Behaviors
{
    public class StretchBehavior : Behavior
    {
        [RequiredComponent]
        private Transform2D transform = null;

        private float cachedScreenWidth;
        private float cachedScreenHeight;

        private ViewportManager viewportManager;

        protected override void Initialize()
        {
            base.Initialize();

            this.viewportManager = WaveServices.ViewportManager;
        }

        protected override void Update(TimeSpan gameTime)
        {
            if ((this.cachedScreenWidth != this.viewportManager.ScreenWidth) || (this.cachedScreenHeight != this.viewportManager.ScreenHeight))
            {
                this.cachedScreenWidth = this.viewportManager.ScreenWidth;
                this.cachedScreenHeight = this.viewportManager.ScreenHeight;

                this.transform.X = this.viewportManager.LeftEdge;
                this.transform.Y = this.viewportManager.TopEdge;
                this.transform.XScale = (this.cachedScreenWidth / this.transform.Rectangle.Width) / this.viewportManager.RatioX;
                this.transform.YScale = (this.cachedScreenHeight / this.transform.Rectangle.Height) / this.viewportManager.RatioY;
            }
        }
    }
}
