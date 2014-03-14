#region File Description
//-----------------------------------------------------------------------------
// Flying Kite
//
// Quickstarter for Wave University Tour 2014.
// Author: Wave Engine Team
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using FlyingKiteProject.Drawables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services; 
#endregion

namespace FlyingKiteProject.Behaviors
{
    public class LinkedRopeBehavior : Behavior
    {
        [RequiredComponent]
        private DrawableCurve2D drawableCurve2D;

        private Transform2D fromTransform;
        private Transform2D toTransform;
        private Vector2 fromOrigin;
        private Vector2 toOrigin;

        public LinkedRopeBehavior(Entity from, Vector2 fromOrigin, Entity to, Vector2 toOrigin)
        {
            this.fromTransform = from.FindComponent<Transform2D>();
            this.toTransform = to.FindComponent<Transform2D>();
            this.fromOrigin = fromOrigin;
            this.toOrigin = toOrigin;
        }

        protected override void Update(TimeSpan gameTime)
        {
            var startPoint = this.GetPointWithOrigin(this.fromTransform, this.fromOrigin);

            var endPoint = this.GetPointWithOrigin(this.toTransform, this.toOrigin);

            this.drawableCurve2D.Curve = new Vector2[] { startPoint, endPoint };
        }

        private Vector2 GetPointWithOrigin(Transform2D entityTransform, Vector2 origin)
        {
            var rotationQuad = Quaternion.CreateFromYawPitchRoll(0, 0, entityTransform.Rotation);

            var destPoint = new Vector2(
                (origin.X * entityTransform.Rectangle.Width) - (entityTransform.Origin.X * entityTransform.Rectangle.Width),
                (origin.Y * entityTransform.Rectangle.Height) - (entityTransform.Origin.Y * entityTransform.Rectangle.Height));

            Vector2.Transform(ref destPoint, ref rotationQuad, out destPoint);

            destPoint += new Vector2(entityTransform.X, entityTransform.Y);

            return destPoint;
        }
    }
}
