using System;
using System.Runtime.Serialization;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;

namespace P2PTank.Behaviors.Cameras
{
    [DataContract]
    public class Player3DCameraBehavior : Behavior
    {
        private Vector3 desiredPosition;
        private Vector3 offset;
        
        [RequiredComponent]
        public Transform3D transform = null;

        [DataMember]
        public float Speed { get; set; }

        [IgnoreDataMember]
        public Transform3D TargetTransform { get; set; }

        [DataMember]
        public bool Follow { get; set; }

        public void Reset(Vector3 position)
        {
            this.desiredPosition = position;
            this.transform.Position = position + this.offset;
            this.transform.LookAt(position);
        }

        protected override void DefaultValues()
        {
            base.DefaultValues();

            this.Speed = 1.0f;
        }

        protected override void Initialize()
        {
            base.Initialize();

            this.desiredPosition = this.transform.Position;
            this.offset = new Vector3(0, 10, 5);
        }

        protected override void Update(TimeSpan gameTime)
        {
            if (this.Follow && this.TargetTransform != null)
            {
                this.desiredPosition = this.TargetTransform.Position;

                if (this.desiredPosition == Vector3.Zero)
                    return;

                if (this.transform.Position != this.desiredPosition)
                {
                    var position = 
                        Vector3.SmoothStep(this.transform.Position - this.offset, this.desiredPosition, this.Speed * (float)gameTime.TotalSeconds);
            
                    this.transform.Position = position + this.offset;
                    this.transform.LookAt(position);
                }
            }
        }
    }
}
