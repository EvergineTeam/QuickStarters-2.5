using System;
using System.Runtime.Serialization;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;

namespace P2PTank.Behaviors
{
    [DataContract]
    public class FixedRotationBehavior : Behavior
    {
        [RequiredComponent]
        private Transform2D transform = null;

        [DataMember]
        public float FixedAngle { get; set; }

        protected override void Update(TimeSpan gameTime)
        {
            this.transform.Rotation = this.FixedAngle;
        }
    }
}
