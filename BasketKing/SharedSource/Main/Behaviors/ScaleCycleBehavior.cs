using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;

namespace BasketKing.Behaviors
{
    [DataContract(Namespace = "BasketKing.Behaviors")]
    public class ScaleCycleBehavior : Behavior
    {
        private double acumulatedTime;

        [RequiredComponent]
        private Transform2D transform = null;

        [DataMember]
        public float MaxScale { get; set; }

        [DataMember]
        public float MinScale { get; set; }

        [DataMember]
        public float Period { get; set; }        

        protected override void DefaultValues()
        {
            base.DefaultValues();

            this.MaxScale = 1.0f;
            this.MinScale = 0.5f; 
            this.Period = 1;
        }

        protected override void Update(TimeSpan gameTime)
        {
            this.acumulatedTime += gameTime.TotalSeconds;

            double cicleValue = (0.5 * Math.Cos(acumulatedTime * this.Period)) + 0.5;
            float scale = this.MinScale + (float)((this.MaxScale - this.MinScale) * cicleValue);

            this.transform.LocalScale = Vector2.One * scale;
        }
    }
}
