#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Helpers;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
using WaveEngine.Framework.UI;
#endregion

namespace OrbitRabbits.Behaviors
{
    [DataContract(Namespace = "OrbitRabbits.Behaviors")]
    public class StretchBehavior : Behavior
    {
        [RequiredComponent]
        private Transform2D transform = null;

        private bool dirtyFlag;

        private ViewportManager viewportManager;
        private Platform platform;

        protected override void DefaultValues()
        {
            base.DefaultValues();
            this.dirtyFlag = true;
        }

        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            this.viewportManager = WaveServices.ViewportManager;
            this.platform = WaveServices.Platform;
            this.platform.OnScreenSizeChanged += this.Platform_OnScreenSizeChanged;
        }

        protected override void DeleteDependencies()
        {
            base.DeleteDependencies();
            this.platform.OnScreenSizeChanged -= this.Platform_OnScreenSizeChanged;
        }

        private void Platform_OnScreenSizeChanged(object sender, SizeEventArgs e)
        {
            this.dirtyFlag = true;
        }

        protected override void Update(TimeSpan gameTime)
        {
            if (this.dirtyFlag)
            {
                this.dirtyFlag = false;


                this.transform.Origin = Vector2.Zero;
                if (this.viewportManager.IsActivated)
                {
                    this.transform.X = this.viewportManager.LeftEdge;
                    this.transform.Y = this.viewportManager.TopEdge;
                    this.transform.XScale = ((this.viewportManager.ScreenWidth / this.transform.Rectangle.Width) / this.viewportManager.RatioX);
                    this.transform.YScale = ((this.viewportManager.ScreenHeight / this.transform.Rectangle.Height) / this.viewportManager.RatioY);
                }
                else
                {
                    this.transform.X = this.RenderManager.ActiveCamera2D.Position.X - (this.platform.ScreenWidth / 2);
                    this.transform.Y = this.RenderManager.ActiveCamera2D.Position.Y - (this.platform.ScreenHeight / 2);
                    this.transform.XScale = ((this.platform.ScreenWidth / this.transform.Rectangle.Width));
                    this.transform.YScale = ((this.platform.ScreenHeight / this.transform.Rectangle.Height));
                }
            }
        }
    }
}
