#region File Description
//-----------------------------------------------------------------------------
// Flying Kite
//
// Quickstarter for Wave University Tour 2014.
// Author: Wave Engine Team
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using FlyingKite.Drawables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
#endregion

namespace FlyingKite.Behaviors
{
    [DataContract]
    public class LinkedRopeBehavior : Behavior
    {
        [RequiredComponent]
        private DrawableCurve2D drawableCurve2D = null;

        [RequiredComponent]
        private Transform2D fromTransform = null;

        private string toEntityPath;
        
        [DataMember]
        private Vector2 toOrigin;

        private Transform2D toTransform;
        
        [DataMember]
        [RenderPropertyAsEntity(new String[] { "WaveEngine.Framework.Graphics.Transform2D" })]
        public string ToEntity
        {
            get
            {
                return this.toEntityPath;
            }

            set
            {
                this.toEntityPath = value;

                if (this.isInitialized)
                {
                    this.RefreshEntities();
                }
            }
        }
        
        public Vector2 ToOrigin
        {
            get { return this.toOrigin; }
            set { this.toOrigin = value; }
        }
        
        protected override void DefaultValues()
        {
            base.DefaultValues();

            this.Family = FamilyType.PriorityBehavior;
        }

        protected override void Initialize()
        {
            base.Initialize();

            this.RefreshEntities();
        }

        protected override void Update(TimeSpan gameTime)
        {
            var startPoint = this.fromTransform.Position;

            var endPoint = this.GetPointWithOrigin(this.toTransform, this.toOrigin);

            this.drawableCurve2D.Curve = new Vector2[] { startPoint, endPoint };
        }

        private void RefreshEntities()
        {
            var toEntity = this.EntityManager.Find(this.toEntityPath);
            
            if (toEntity != null)
            {
                this.toTransform = toEntity.FindComponent<Transform2D>();
            }
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
