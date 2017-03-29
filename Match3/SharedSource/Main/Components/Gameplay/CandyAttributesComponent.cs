using Match3.Gameboard;
using System;
using WaveEngine.Framework;

namespace Match3.Components.Gameplay
{
    public class CandyAttributesComponent : Component
    {
        public CandyAnimationBehavior Animator
        {
            get
            {
                return this.Owner?.FindComponent<CandyAnimationBehavior>();
            }
        }

        public Coordinate Coordinate { get; set; }
        
        public CandyTypes Type { get; set; }

        public CandyColors Color { get; set; }

        public CandyAttributesComponent(Coordinate coord, Candy properties)
        {
            this.Coordinate = coord;
            this.Type = properties.Type;
            this.Color = properties.Color;
        }
    }
}
