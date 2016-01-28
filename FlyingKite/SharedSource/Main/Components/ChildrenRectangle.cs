#region File Description
//-----------------------------------------------------------------------------
// Flying Kite
//
// Quickstarter for Wave University Tour 2014.
// Author: Wave Engine Team
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
#endregion

namespace FlyingKite.Components
{
    [DataContract]
    public class ChildrenRectangle : Component
    {
        [RequiredComponent]
        private Transform2D transform2D = null;

        protected override void Initialize()
        {
            base.Initialize();

            this.Owner.EntityInitialized += (s, o) =>
            {
                this.transform2D.Rectangle = this.GetTotalRectangle(this.Owner, null);
            };
        }

        private RectangleF GetTotalRectangle(Entity entity, Transform2D parentTransform2D)
        {
            RectangleF result = RectangleF.Empty;
            var entityTransform = entity.FindComponent<Transform2D>();

            if (entityTransform != null)
            {
                if (parentTransform2D != null)
                {
                    result = entityTransform.Rectangle;

                    result.Offset(
                        entityTransform.X - (entityTransform.Rectangle.Width * entityTransform.Origin.X),
                        entityTransform.Y - (entityTransform.Rectangle.Height * entityTransform.Origin.Y));

                    result.Offset(
                        -1 * (parentTransform2D.X - (parentTransform2D.Rectangle.Width * parentTransform2D.Origin.X)),
                        -1 * (parentTransform2D.Y - (parentTransform2D.Rectangle.Height * parentTransform2D.Origin.Y)));
                }

                foreach (var child in entity.ChildEntities)
                {
                    var childTotalRectangle = this.GetTotalRectangle(child, entityTransform);

                    RectangleF.Union(ref result, ref childTotalRectangle, out result);
                }
            }

            return result;
        }
    }
}
