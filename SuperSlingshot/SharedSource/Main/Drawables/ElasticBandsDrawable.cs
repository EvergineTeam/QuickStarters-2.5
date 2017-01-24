using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Graphics;

namespace SuperSlingshot.Drawables
{
    [DataContract]
    public class ElasticBandsDrawable : Drawable2D
    {
        private Vector2 center;

        [DataMember]
        public Vector2 FixedFrontPoint { get; set; }

        [DataMember]
        public Vector2 FixedBackPoint { get; set; }

        [DataMember]
        public Color Color { get; set; }

        [IgnoreDataMember]
        public Transform2D TargetTransform { get; set; }

        public override void Draw(TimeSpan gameTime)
        {
            if (this.TargetTransform != null)
            {
                center = (FixedFrontPoint + FixedBackPoint) / 2;
                var diff = this.TargetTransform.Position - center;
                diff.Normalize();
                var offset = diff * 40;

                this.RenderManager.LineBatch2D.DrawLine(this.FixedBackPoint, this.TargetTransform.Position + offset, Color.Red, this.TargetTransform.DrawOrder + 1);
                this.RenderManager.LineBatch2D.DrawLine(this.FixedFrontPoint, this.TargetTransform.Position + offset, Color.Red, this.TargetTransform.DrawOrder - 1);
            }
        }

        protected override void Dispose(bool disposing)
        {
        }
    }
}
