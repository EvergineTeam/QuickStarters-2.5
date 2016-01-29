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
using WaveEngine.Framework.Managers;
using WaveEngine.Framework.Services;
using WaveEngine.Framework.UI;
#endregion

namespace SuperSquid.Behaviors
{
    [DataContract(Namespace = "SuperSquid.Behaviors")]
    public class StretchBehavior : Behavior
    {
        [RequiredComponent]
        private Transform2D transform = null;

        private bool dirtyFlag;

        private VirtualScreenManager virtualScreenManager;
        private Platform platform;

        protected override void DefaultValues()
        {
            base.DefaultValues();
            this.dirtyFlag = true;
        }

        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            this.virtualScreenManager = this.Owner.Scene.VirtualScreenManager;
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
                this.transform.X = this.virtualScreenManager.LeftEdge;
                this.transform.Y = this.virtualScreenManager.TopEdge;
                this.transform.XScale = ((this.virtualScreenManager.ScreenWidth / this.transform.Rectangle.Width) / this.virtualScreenManager.RatioX);
                this.transform.YScale = ((this.virtualScreenManager.ScreenHeight / this.transform.Rectangle.Height) / this.virtualScreenManager.RatioY);
            }
        }
    }
}
