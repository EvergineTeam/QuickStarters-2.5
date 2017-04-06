using Match3.Components.Gameplay;
using Match3.Gameboard;
using Match3.Services;
using WaveEngine.Common.Math;

namespace Match3.Helpers
{
    public static class GameLogicExtensions
    {
        public static Vector2 CalculateCandyPostion(this GameLogic gameLogic, Coordinate coordinate)
        {
            var horizontalOffset = (gameLogic.BoardSizeM * 0.5f) - 0.5f;
            var verticalOffset = (gameLogic.BoardSizeN * 0.5f) - 0.5f;

            return new Vector2((coordinate.X - horizontalOffset), (coordinate.Y - verticalOffset)) * GameboardContent.DistanceBtwCandies;
        }
    }
}
