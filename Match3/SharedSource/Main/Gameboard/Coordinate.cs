using System;

namespace Match3.Gameboard
{
    public struct Coordinate
    {
        public int X;

        public int Y;

        public override string ToString()
        {
            return $"[X={this.X}, Y={this.Y}]";
        }

        public static bool operator ==(Coordinate c1, Coordinate c2)
        {
            return c1.X == c2.X && 
                   c1.Y == c2.Y;
        }

        public static bool operator !=(Coordinate c1, Coordinate c2)
        {
            return c1.X != c2.X ||
                   c1.Y != c2.Y;
        }

        public Coordinate Calculate(CandyMoves move)
        {
            var otherCandyPosition = this;
            switch (move)
            {
                case CandyMoves.Left:
                    otherCandyPosition.X--;
                    break;
                case CandyMoves.Right:
                    otherCandyPosition.X++;
                    break;
                case CandyMoves.Top:
                    otherCandyPosition.Y--;
                    break;
                case CandyMoves.Bottom:
                    otherCandyPosition.Y++;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("The indicated candy move is not valid.");
            }

            return otherCandyPosition;
        }
    }
}