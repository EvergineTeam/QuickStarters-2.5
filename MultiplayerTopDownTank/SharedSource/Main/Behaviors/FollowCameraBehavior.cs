using System;
using System.Runtime.Serialization;
using WaveEngine.Common.Math;

namespace MultiplayerTopDownTank.Behaviors
{
    [DataContract]
    public class FollowCameraBehavior : CameraBehavior
    {
        private Vector2 desiredPosition;
        private Vector2 currentPosition;

        protected override void Initialize()
        {
            base.Initialize();
            this.desiredPosition = this.transform.Position;
        }

        protected override void CameraUpdate(TimeSpan gameTime)
        {
            if (this.Follow && this.TargetTransform != null)
            {
                var newDesiredPosition = this.TargetTransform.Position;
                if (newDesiredPosition.X < this.pixelLimitMin.X)
                {
                    newDesiredPosition.X = this.pixelLimitMin.X;
                }
                else if(newDesiredPosition.X > this.pixelLimitMax.X)
                {
                    newDesiredPosition.X = this.pixelLimitMax.X;
                }


                if(newDesiredPosition.Y < this.pixelLimitMin.Y)
                {
                    newDesiredPosition.Y = this.pixelLimitMin.Y;
                }
                else if (newDesiredPosition.Y > this.pixelLimitMax.Y)
                {
                    newDesiredPosition.Y = this.pixelLimitMax.Y;
                }

                this.desiredPosition = newDesiredPosition;
                this.transform.Position = Vector2.SmoothStep(this.currentPosition, this.desiredPosition, this.Speed * (float)gameTime.TotalSeconds);
                this.currentPosition = this.transform.Position;
            }
        }
    }
}