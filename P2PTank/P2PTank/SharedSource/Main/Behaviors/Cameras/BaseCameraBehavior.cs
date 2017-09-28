using System;
using System.Runtime.Serialization;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;

namespace P2PTank.Behaviors.Cameras
{
    [DataContract]
    public class BaseCameraBehavior : Behavior
    {
        [RequiredComponent]
        protected Transform2D cameraTransform = null;

        [DataMember]
        public float Speed { get; set; }

        [DataMember]
        public bool Follow { get; set; }

        [IgnoreDataMember]
        public Transform2D TargetTransform { get; private set; }

        protected override void DefaultValues()
        {
            base.DefaultValues();

            this.Speed = 1.0f;
        }

        public void SetTarget(Transform2D target)
        {
            this.TargetTransform = target;
        }

        protected override void Update(TimeSpan gameTime)
        {
            this.ChildUpdate(gameTime);
        }

        protected virtual void ChildUpdate(TimeSpan gameTime)
        {
        }
    }
}
