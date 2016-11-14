#region Using Statements
using System;
using System.Runtime.Serialization;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Diagnostic;
using WaveEngine.Framework.Physics2D;
#endregion

namespace SuperSlingshot.Behaviors
{
    [DataContract]
    public class CurbCameraBehavior : CameraBehavior
    {
        private Vector2 desiredPosition;
        private Vector2 currentPosition;

        private float pixelBottomFrameSize;
        private float pixelTopFrameSize;
        private float pixelLeftFrameSize;
        private float pixelRightFrameSize;

        [DataMember]
        [RenderPropertyAsFInput(MinLimit = 0, MaxLimit = 0.5f)]
        public float BottomFrameSizePercent { get; set; }

        [DataMember]
        [RenderPropertyAsFInput(MinLimit = 0, MaxLimit = 0.5f)]
        public float TopFrameSizePercent { get; set; }

        [DataMember]
        [RenderPropertyAsFInput(MinLimit = 0, MaxLimit = 0.5f)]
        public float RightFrameSizePercent { get; set; }

        [DataMember]
        [RenderPropertyAsFInput(MinLimit = 0, MaxLimit = 0.5f)]
        public float LeftFrameSizePercent { get; set; }

        protected override void Initialize()
        {
            base.Initialize();
            this.desiredPosition = this.transform.Position;
        }

        protected override void CameraUpdate(TimeSpan gameTime)
        {
            if (this.Follow && this.TargetTransform != null)
            {
                var floatGameTime = (float)gameTime.TotalSeconds;

                var delta = this.CalculateCameraDelta();
                if (delta != Vector2.Zero)
                {
                    this.desiredPosition = this.CalculateCameraBounds(this.desiredPosition + delta);
                    this.transform.Position = Vector2.SmoothStep(this.currentPosition, this.desiredPosition, this.Speed * floatGameTime);
                    this.currentPosition = this.transform.Position;
                }
            }

            Labels.Add(this.Owner.Name + " position", this.transform.Position);
        }

        private Vector2 CalculateCameraBounds(Vector2 position)
        {
            return Vector2.Clamp(position, this.pixelLimitMin, this.pixelLimitMax);
        }

        protected override void ChildRefreshCamera()
        {
            Vector2 screenSize = new Vector2((this.virtualScreenManager.RightEdge - this.virtualScreenManager.LeftEdge),
                (this.virtualScreenManager.TopEdge - this.virtualScreenManager.BottomEdge));

            this.pixelBottomFrameSize = Math.Abs(screenSize.Y * this.BottomFrameSizePercent);
            this.pixelTopFrameSize = Math.Abs(screenSize.Y * this.TopFrameSizePercent);
            this.pixelLeftFrameSize = Math.Abs(screenSize.X * this.LeftFrameSizePercent);
            this.pixelRightFrameSize = Math.Abs(screenSize.X * this.RightFrameSizePercent);
        }

        private Vector2 CalculateCameraDelta()
        {
            var delta = Vector2.Zero;
            var position = this.TargetTransform.Position.ToVector3(0);
            var projected = this.camera2D.Project(ref position);

            if (projected.X > this.vsm.RightEdge - this.pixelRightFrameSize)
            {
                delta.X = projected.X - (this.vsm.RightEdge - this.pixelRightFrameSize);
            }
            else if (projected.X < this.vsm.LeftEdge + this.pixelLeftFrameSize)
            {
                delta.X = projected.X - (this.vsm.LeftEdge + this.pixelLeftFrameSize);
            }

            if (projected.Y > this.vsm.BottomEdge - this.pixelBottomFrameSize)
            {
                delta.Y = projected.Y - (this.vsm.BottomEdge - this.pixelBottomFrameSize);
            }
            else if (projected.Y < this.vsm.TopEdge + this.pixelTopFrameSize)
            {
                delta.Y = projected.Y - (this.vsm.TopEdge + this.pixelTopFrameSize);
            }

            return delta;
        }
    }
}
