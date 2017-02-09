using Match3.Gameboard;
using System;
using WaveEngine.Framework;

namespace Match3.Components.Gameplay
{
    public class CandyAttributesComponent : Component
    {
        private Coordinate coordinate;

        public Coordinate Coordinate
        {
            get
            {
                return this.coordinate;
            }

            set
            {
                if (this.coordinate != value)
                {
                    this.coordinate = value;

                    this.OnCoordinateChanged?.Invoke(this, this.coordinate);
                }
            }
        }

        public EventHandler<Coordinate> OnCoordinateChanged;
    }
}
