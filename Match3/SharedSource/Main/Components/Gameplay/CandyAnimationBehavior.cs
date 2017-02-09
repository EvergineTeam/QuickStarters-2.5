using System;
using System.Runtime.Serialization;
using Match3.Gameboard;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;

namespace Match3.Components.Gameplay
{
    [DataContract]
    public class CandyAnimationBehavior : Behavior, IDisposable
    {
        [RequiredComponent]
        protected Transform2D transform2D;

        [RequiredComponent]
        protected CandyAttributesComponent candyAttributes;
        
        private Vector2 destinationPosition;

        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            this.candyAttributes.OnCoordinateChanged -= this.CandyAttributes_OnCoordinateChanged;
            this.candyAttributes.OnCoordinateChanged += this.CandyAttributes_OnCoordinateChanged;
        }

        protected override void DeleteDependencies()
        {
            this.DeleteInternalDependencies();

            base.DeleteDependencies();
        }

        public void Dispose()
        {
            this.DeleteInternalDependencies();
        }

        private void DeleteInternalDependencies()
        {
            if (this.candyAttributes != null)
            {
                this.candyAttributes.OnCoordinateChanged -= this.CandyAttributes_OnCoordinateChanged;
                this.candyAttributes.OnCoordinateChanged += this.CandyAttributes_OnCoordinateChanged;
            }
        }
        
        private void CandyAttributes_OnCoordinateChanged(object sender, Coordinate e)
        {
            //this.destinationPosition = 
        }

        protected override void Update(TimeSpan gameTime)
        {
            if (this.destinationPosition != this.transform2D.LocalPosition)
            {
                this.transform2D.LocalPosition = Vector2.Lerp(this.transform2D.LocalPosition, this.destinationPosition, 0.5f);
            }
        }
    }
}
