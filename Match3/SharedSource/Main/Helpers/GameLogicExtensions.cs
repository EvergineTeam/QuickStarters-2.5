using Match3.Gameboard;
using Match3.Services;
using System;
using System.Collections.Generic;
using System.Text;
using WaveEngine.Common.Math;

namespace Match3.Helpers
{
    public static class GameLogicExtensions
    {
        public const int DistanceBtwItems = 150;

        public static Vector2 CalculateCandyPostion(this GameLogic gameLogic, Coordinate coordinate)
        {
            var horizontalOffset = (gameLogic.BoardSizeM * 0.5f) - 0.5f;
            var verticalOffset = (gameLogic.BoardSizeN * 0.5f) - 0.5f;

            return new Vector2((coordinate.X - horizontalOffset) * DistanceBtwItems, (coordinate.Y - verticalOffset) * DistanceBtwItems);
        }
    }
}
