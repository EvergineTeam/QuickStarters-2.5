#region Using Statements
using System;
using System.Runtime.Serialization;
using WaveEngine.Common.Math;
#endregion

namespace SuperSlingshot.Behaviors
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
                this.desiredPosition = this.TargetTransform.Position;

                this.transform.Position = Vector2.SmoothStep(this.currentPosition, this.desiredPosition, this.Speed * (float)gameTime.TotalSeconds);
                this.currentPosition = this.transform.Position;
            }
        }
    }
}
