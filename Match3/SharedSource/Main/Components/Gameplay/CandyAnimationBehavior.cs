using System;
using System.Runtime.Serialization;
using Match3.Gameboard;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using Match3.Helpers;
using Match3.Services;

namespace Match3.Components.Gameplay
{
    [DataContract]
    public class CandyAnimationBehavior : Behavior
    {
        [RequiredComponent]
        protected Transform2D transform2D;

        [RequiredComponent]
        protected CandyAttributesComponent candyAttributes;

        [RequiredComponent]
        protected CandyTouchComponent candyTouchComponent;

        private Vector2 destinationPosition;
        

        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            this.candyAttributes.OnCoordinateChanged += this.CandyAttributes_OnCoordinateChanged;
        }

        protected override void Initialize()
        {
            base.Initialize();

            this.destinationPosition = this.transform2D.LocalPosition;
        }

        private void CandyAttributes_OnCoordinateChanged(object sender, Coordinate coordinate)
        {
            var gameLogic = CustomServices.GameLogic;
            this.destinationPosition = gameLogic.CalculateCandyPostion(coordinate);
        }

        protected override void Update(TimeSpan gameTime)
        {
            var isAnimating = this.destinationPosition != this.transform2D.LocalPosition && !this.candyTouchComponent.IsPressed;
            this.candyAttributes.IsAnimating = isAnimating;

            if (isAnimating)
            {
                this.transform2D.LocalPosition = Vector2.Lerp(this.transform2D.LocalPosition, this.destinationPosition, 0.5f);
            }
        }
    }
}
