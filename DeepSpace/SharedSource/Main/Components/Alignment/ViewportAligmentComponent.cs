using System;
using System.Runtime.Serialization;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
using WaveEngine.Framework.UI;

namespace DeepSpace.Components.Alignment
{
    [DataContract]
    public class ViewportAligmentComponent : Component, IDisposable
    {
        [RequiredComponent]
        protected Transform2D transform2D;

        [DataMember]
        public bool IsGlobalPosition { get; set; }

        [DataMember]
        public Vector2 HorizontalMargin { get; set; }

        [DataMember]
        public Vector2 VerticalMargin { get; set; }

        [DataMember]
        public float ProportionalMarginOffset { get; set; }

        [DataMember]
        public bool UseTextureSize { get; set; }

        [DataMember]
        public VerticalAlignment VerticalAlignment { get; set; }

        [DataMember]
        public HorizontalAlignment HorizontalAlignment { get; set; }

        public ViewportAligmentComponent() : base("ViewportAligmentComponent")
        {
        }

        protected override void DefaultValues()
        {
            this.UseTextureSize = true;
            this.VerticalAlignment = VerticalAlignment.Stretch;
            this.HorizontalAlignment = HorizontalAlignment.Stretch;
        }

        protected override void Initialize()
        {
            base.Initialize();
            
            WaveServices.Platform.OnScreenSizeChanged += this.OnScreenSizeChanged;
            this.Resize();
        }

        private void OnScreenSizeChanged(object sender, WaveEngine.Common.Helpers.SizeEventArgs e)
        {
            this.Resize();
        }

        public void Resize()
        {
            float width = 0, height = 0;

            var virtualScreenManager = this.Owner.Scene.VirtualScreenManager;

            var virtualScreenWidth = virtualScreenManager.RightEdge - virtualScreenManager.LeftEdge;
            var virtualScreenHeight = virtualScreenManager.BottomEdge - virtualScreenManager.TopEdge;
            var horizontalRatio = MathHelper.Max(0, virtualScreenWidth / virtualScreenManager.VirtualWidth) - 1;
            var verticalRatio = MathHelper.Max(0, virtualScreenHeight / virtualScreenManager.VirtualHeight) - 1;
            var horizontalOffset = this.ProportionalMarginOffset * horizontalRatio;
            var verticalOffset = this.ProportionalMarginOffset * horizontalRatio;

            if (this.UseTextureSize)
            {
                width = this.transform2D.Rectangle.Width * this.transform2D.XScale;
                height = this.transform2D.Rectangle.Height * this.transform2D.YScale;
            }

            if (this.HorizontalAlignment == HorizontalAlignment.Left)
            {
                this.SetXPosition(virtualScreenManager.LeftEdge + (width * this.transform2D.Origin.X) + this.HorizontalMargin.X + horizontalOffset);
            }
            else if (this.HorizontalAlignment == HorizontalAlignment.Right)
            {
                this.SetXPosition(virtualScreenManager.RightEdge - (width * (1 - this.transform2D.Origin.X)) - this.HorizontalMargin.Y - horizontalOffset);
            }
            else if (this.HorizontalAlignment == HorizontalAlignment.Center)
            {
                this.SetXPosition((virtualScreenManager.VirtualWidth * 0.5f) + this.HorizontalMargin.X + this.HorizontalMargin.Y + horizontalOffset);
            }

            if (this.VerticalAlignment == VerticalAlignment.Top)
            {
                this.SetYPosition(virtualScreenManager.TopEdge + (height * this.transform2D.Origin.Y) + this.VerticalMargin.X + verticalOffset);
            }
            else if (this.VerticalAlignment == VerticalAlignment.Bottom)
            {
                this.SetYPosition(virtualScreenManager.BottomEdge - (height * (1 - this.transform2D.Origin.Y)) - this.VerticalMargin.Y - verticalOffset);
            }
            else if (this.VerticalAlignment == VerticalAlignment.Center)
            {
                this.SetYPosition((virtualScreenManager.VirtualHeight * 0.5f) + this.VerticalMargin.X + this.VerticalMargin.Y + verticalOffset);
            }
        }

        private void SetXPosition(float x)
        {
            if (this.IsGlobalPosition)
            {
                this.transform2D.X = x;
            }
            else
            {
                this.transform2D.LocalX = x;
            }
        }

        private void SetYPosition(float y)
        {
            if (this.IsGlobalPosition)
            {
                this.transform2D.Y = y;
            }
            else
            {
                this.transform2D.LocalY = y;
            }
        }

        public void Dispose()
        {
            WaveServices.Platform.OnScreenSizeChanged -= this.OnScreenSizeChanged;
        }
    }
}
