using Match3.Factories;
using Match3.Gameboard;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;

namespace Match3.Components.Gameplay
{
    [DataContract]
    public class GameboardContent : Component
    {
        public const int DistanceBtwItems = 150;

        [RequiredComponent]
        protected Transform2D transform;

        private Board board;

        protected override void Initialize()
        {
            base.Initialize();
        }

        public void RegenerateGameboard(Board board)
        {
            this.board = board;

            foreach (var child in this.Owner.ChildEntities.ToList())
            {
                var candyTouch = child.FindComponent<CandyTouchComponent>();

                if (candyTouch != null)
                {
                    candyTouch.OnMoveOperation -= this.CandyTouch_OnMoveOperation;
                }

                this.Owner.RemoveChild(child.Name);
            }

            for (int i = 0; i < board.SizeM; i++)
            {
                for (int j = 0; j < board.SizeN; j++)
                {
                    var coord = new Coordinate() { X = i, Y = j };
                    Vector2 position = CalculateCandyPostion(coord);
                    var candy = board.CurrentStatus[i][j];

                    var candyEntity = GameplayFactory.CreateCandy(this.Owner.Scene, position, candy.Type, candy.Color);
                    candyEntity.Name += i + "_" + j;
                    var candyTouch = new CandyTouchComponent();
                    candyTouch.OnMoveOperation += this.CandyTouch_OnMoveOperation;
                    candyEntity.AddComponent(candyTouch)
                               .AddComponent(new CandyAttributesComponent() { Coordinate = coord });
                    this.Owner.AddChild(candyEntity);
                }
            }
        }

        private Vector2 CalculateCandyPostion(Coordinate coordinate)
        {
            var horizontalOffset = (this.board.SizeM * 0.5f) - 0.5f;
            var verticalOffset = (this.board.SizeN * 0.5f) - 0.5f;

            return new Vector2((coordinate.X - horizontalOffset) * DistanceBtwItems, (coordinate.Y - verticalOffset) * DistanceBtwItems);
        }

        private void CandyTouch_OnMoveOperation(object sender, CandyMoves move)
        {
            var candyTouch = (CandyTouchComponent)sender;
            var candyAttributes = candyTouch.Owner.FindComponent<CandyAttributesComponent>();
            var coordinate = candyAttributes.Coordinate;

            Debug.WriteLine("Move [{0},{1}] {2}!", coordinate.X, coordinate.Y, move);
            this.board.Move(coordinate, move);
            this.RegenerateGameboard(this.board);
        }
    }
}
