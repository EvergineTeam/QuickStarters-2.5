#region File Description
//-----------------------------------------------------------------------------
// Flying Kite
//
// Quickstarter for Wave University Tour 2014.
// Author: Wave Engine Team
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using WaveEngine.Common.Helpers;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Managers;
using WaveEngine.Framework.Services;
#endregion

namespace FlyingKite.Components
{
    [DataContract]
    public class StretchComponent : Component, IDisposable
    {
        [RequiredComponent]
        private Transform2D transform = null;
        
        private VirtualScreenManager virtualScreenManager;

        private Platform platform;

        protected override void DefaultValues()
        {
            base.DefaultValues();
        }

        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            this.virtualScreenManager = this.Owner.Scene.VirtualScreenManager;
            this.platform = WaveServices.Platform;

            this.platform.OnScreenSizeChanged -= this.Platform_OnScreenSizeChanged;
            this.platform.OnScreenSizeChanged += this.Platform_OnScreenSizeChanged;
        }

        protected override void Initialize()
        {
            base.Initialize();

            this.AjustTransform();
        }

        private void Platform_OnScreenSizeChanged(object sender, SizeEventArgs e)
        {
            this.AjustTransform();
        }

        private void AjustTransform()
        {
            this.transform.Origin = Vector2.Zero;

            this.transform.X = this.virtualScreenManager.LeftEdge;
            this.transform.Y = this.virtualScreenManager.TopEdge;
            this.transform.XScale = ((this.virtualScreenManager.ScreenWidth / this.transform.Rectangle.Width) / this.virtualScreenManager.RatioX);
            this.transform.YScale = ((this.virtualScreenManager.ScreenHeight / this.transform.Rectangle.Height) / this.virtualScreenManager.RatioY);
        }

        public void Dispose()
        {
            this.platform.OnScreenSizeChanged -= this.Platform_OnScreenSizeChanged;
        }
    }
}