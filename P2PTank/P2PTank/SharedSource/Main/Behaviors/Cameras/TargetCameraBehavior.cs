using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using WaveEngine.Common.Math;
using WaveEngine.Framework;

namespace P2PTank.Behaviors.Cameras
{
    [DataContract]
    public class TargetCameraBehavior : BaseCameraBehavior
    {
        private Vector2 desiredPosition;
        private Vector2 currentPosition;

        protected override void Initialize()
        {
            base.Initialize();

            this.desiredPosition = this.cameraTransform.Position;
        }

        protected override void ChildUpdate(TimeSpan gameTime)
        {
            base.ChildUpdate(gameTime);

            if (this.Follow && this.TargetTransform != null)
            {
                var newDesiredPosition = this.TargetTransform.Position;
               
                this.desiredPosition = newDesiredPosition;
                this.cameraTransform.Position = Vector2.SmoothStep(this.currentPosition, this.desiredPosition, this.Speed * (float)gameTime.TotalSeconds);
                this.currentPosition = this.cameraTransform.Position;
            }
        }
    }
}
