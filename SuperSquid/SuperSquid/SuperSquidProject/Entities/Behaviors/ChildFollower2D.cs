#region File Description
//-----------------------------------------------------------------------------
// Super Squid
//
// Quickstarter for Wave University Tour 2014.
// Author: Wave Engine Team
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using SuperSquidProject.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics; 
#endregion

namespace SuperSquidProject.Entities.Behaviors
{
    public class ChildFollower2D : Behavior
    {
        [RequiredComponent]
        private Transform2D transform2D;

        private Vector2 initialPosition;

        private Transform2D parentTransform2D;

        protected override void Initialize()
        {
            base.Initialize();

            if (this.Owner.Parent == null)
            {
                throw new InvalidOperationException("ChildFollower2D component owner needs a parent entity.");
            }

            this.parentTransform2D = this.Owner.Parent.FindComponent<Transform2D>();

            this.initialPosition = new Vector2(
                this.transform2D.X - this.parentTransform2D.X,
                this.transform2D.Y - this.parentTransform2D.Y);
        }

        protected override void Update(TimeSpan gameTime)
        {
            var newPosition = Utils.RotateVectorAroundPoint(this.initialPosition, Vector2.Zero, this.parentTransform2D.Rotation);

            this.transform2D.X = newPosition.X;
            this.transform2D.Y = newPosition.Y;
        }
    }
}
