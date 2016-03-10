using System;
using System.Runtime.Serialization;
using WaveEngine.Common.Helpers;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Managers;
using WaveEngine.Framework.Services;

namespace DeepSpace.Components.Alignment
{
    [DataContract]
    public class BackgroundStretchComponent : Component
    {
        [RequiredComponent]
        protected Transform2D transform;

        public BackgroundStretchComponent() : base("BackgroundStretchComponent")
        {
        }

        protected override void Initialize()
        {
            base.Initialize();

            WaveServices.Platform.OnScreenSizeChanged += this.OnScreenSizeChanged;
            this.Resize();
        }

        private void Resize()
        {
            var virtualScreenManager = this.Owner.Scene.VirtualScreenManager;
            this.transform.Origin = Vector2.Zero;
            this.transform.X = virtualScreenManager.LeftEdge;
            this.transform.Y = virtualScreenManager.TopEdge;
            this.transform.XScale = (virtualScreenManager.ScreenWidth / this.transform.Rectangle.Width) / virtualScreenManager.RatioX;
            this.transform.YScale = (virtualScreenManager.ScreenHeight / this.transform.Rectangle.Height) / virtualScreenManager.RatioY;
        }

        private void OnScreenSizeChanged(object sender, SizeEventArgs e)
        {
            this.Resize();
        }
    }
}
