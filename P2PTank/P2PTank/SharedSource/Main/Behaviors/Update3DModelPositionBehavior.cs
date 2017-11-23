using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Diagnostic;
using WaveEngine.Framework.Graphics;

namespace P2PTank.Behaviors
{
    [DataContract]
    public class Update3DModelPositionBehavior : Behavior
    {
        [RequiredComponent]
        private Transform2D transform = null;

        private Transform3D childEntityTransform3D = null;

        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            this.childEntityTransform3D = this.Owner.FindChild("cube").FindComponent<Transform3D>();
        }

        protected override void Update(TimeSpan gameTime)
        {
            var pos3D = this.childEntityTransform3D.Position;
            var pos2D = this.transform.Position;


            pos3D.X = -pos2D.X;
            pos3D.Z = -pos2D.Y;

            pos3D = pos3D / 50;
            Labels.Add("2Dto3D", pos3D);

            
            this.childEntityTransform3D.Position = pos3D;
            this.childEntityTransform3D.Rotation = -Vector3.UnitY * this.transform.Rotation;
        }
    }
}
