using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using WaveEngine.Common.Helpers;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;

namespace P2PTank.Behaviors.Cameras
{
    [DataContract]
    public class TargetCameraBehavior : BaseCameraBehavior
    {
        [RequiredComponent]
        private Transform2D transform = null;

        private Vector2 desiredPosition;
        private Vector2 currentPosition;

        protected Vector2 pixelLimitMin;
        protected Vector2 pixelLimitMax;

        protected override void Initialize()
        {
            base.Initialize();

            this.desiredPosition = this.cameraTransform.Position;

            var platform = WaveServices.Platform;
            platform.OnScreenSizeChanged += this.OnScreenSizeChanged;
        }

        private void OnScreenSizeChanged(object sender, SizeEventArgs e)
        {
            this.RefreshCameraLimits();
        }

        protected override void ChildUpdate(TimeSpan gameTime)
        {
            base.ChildUpdate(gameTime);

            if (this.Follow && this.TargetTransform != null)
            {
                var newDesiredPosition = this.TargetTransform.Position;
                if (newDesiredPosition.X < this.pixelLimitMin.X)
                {
                    newDesiredPosition.X = this.pixelLimitMin.X;
                }
                else if (newDesiredPosition.X > this.pixelLimitMax.X)
                {
                    newDesiredPosition.X = this.pixelLimitMax.X;
                }


                if (newDesiredPosition.Y < this.pixelLimitMin.Y)
                {
                    newDesiredPosition.Y = this.pixelLimitMin.Y;
                }
                else if (newDesiredPosition.Y > this.pixelLimitMax.Y)
                {
                    newDesiredPosition.Y = this.pixelLimitMax.Y;
                }

                this.desiredPosition = newDesiredPosition;

                var posi = Vector2.SmoothStep(this.currentPosition, this.desiredPosition, this.Speed * (float)gameTime.TotalSeconds);
                this.cameraTransform.Position = (Vector2.UnitX * (int)posi.X) + (Vector2.UnitY * (int)posi.Y);

                this.currentPosition = this.cameraTransform.Position;
            }
        }

        private Vector2 min;
        private Vector2 max;

        public void SetLimits(Vector2 min, Vector2 max)
        {
            this.min = min;
            this.max = max;
        }

        public void RefreshCameraLimits()
        {
            var virtualScreenManager = this.Owner.Scene.VirtualScreenManager;

            Vector2 halfScreenSize =
                new Vector2((virtualScreenManager.RightEdge - virtualScreenManager.LeftEdge) / 2,
                (virtualScreenManager.TopEdge - virtualScreenManager.BottomEdge) / 2);

            this.pixelLimitMax.X = this.max.X - (halfScreenSize.X * this.transform.XScale);
            this.pixelLimitMax.Y = this.max.Y + (halfScreenSize.Y * this.transform.YScale);
            this.pixelLimitMin.X = this.min.X + (halfScreenSize.X * this.transform.XScale);
            this.pixelLimitMin.Y = this.min.Y - (halfScreenSize.Y * this.transform.YScale);

            if (this.pixelLimitMin.Y > this.pixelLimitMax.Y)
            {
                this.pixelLimitMin.Y = this.pixelLimitMax.Y;
            }

            if (this.pixelLimitMin.X > this.pixelLimitMax.X)
            {
                var middle = (this.pixelLimitMin.X - this.pixelLimitMax.X) / 2;
                this.pixelLimitMin.X = middle;
                this.pixelLimitMax.X = middle;
            }
        }

        public void Dispose()
        {
            var platform = WaveServices.Platform;
            platform.OnScreenSizeChanged -= OnScreenSizeChanged;
        }
    }
}
