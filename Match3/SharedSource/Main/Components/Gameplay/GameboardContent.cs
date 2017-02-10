using Match3.Factories;
using Match3.Gameboard;
using Match3.Helpers;
using Match3.Services;
using Match3.Services.Navigation;
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
        [RequiredComponent]
        protected Transform2D transform;

        private bool isMoving;
        private GameLogic gameLogic;

        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();
            this.gameLogic = CustomServices.GameLogic;
        }

        protected override void DeleteDependencies()
        {
            base.DeleteDependencies();
            this.gameLogic = null;
        }

        protected override void Initialize()
        {
            base.Initialize();
            this.RegenerateGameboard();
        }

        private void GameLogicGameFinished(object sender, System.EventArgs e)
        {
            CustomServices.NavigationService.Navigate(NavigateCommands.DefaultForward);
        }

        public void RegenerateGameboard()
        {
            foreach (var child in this.Owner.ChildEntities.ToList())
            {
                var candyTouch = child.FindComponent<CandyTouchComponent>();

                if (candyTouch != null)
                {
                    candyTouch.OnMoveOperation -= this.CandyTouch_OnMoveOperation;
                }

                this.Owner.RemoveChild(child.Name);
            }
            
            for (int i = 0; i < this.gameLogic.BoardSizeM; i++)
            {
                for (int j = 0; j < this.gameLogic.BoardSizeN; j++)
                {
                    var coord = new Coordinate() { X = i, Y = j };
                    var position = this.gameLogic.CalculateCandyPostion(coord);
                    var candy = this.gameLogic.CurrentCandies[i][j];

                    var candyEntity = GameplayFactory.CreateCandy(this.Owner.Scene, position, candy.Type, candy.Color);
                    candyEntity.Name += i + "_" + j;
                    var candyTouch = new CandyTouchComponent();
                    candyTouch.OnMoveOperation += this.CandyTouch_OnMoveOperation;
                    candyEntity.AddComponent(candyTouch)
                               .AddComponent(new CandyAttributesComponent() { Coordinate = coord })
                               .AddComponent(new CandyAnimationBehavior());
                    this.Owner.AddChild(candyEntity);
                }
            }
        }
        
        private void CandyTouch_OnMoveOperation(object sender, CandyMoves move)
        {
            if (this.isMoving)
            {
                return;
            }

            this.isMoving = true;
            var candyTouch = (CandyTouchComponent)sender;
            var candyAttributes = candyTouch.Owner.FindComponent<CandyAttributesComponent>();
            var coordinate = candyAttributes.Coordinate;

            Debug.WriteLine("Move [{0},{1}] {2}!", coordinate.X, coordinate.Y, move);
            var operations = this.gameLogic.Move(coordinate, move);

            //TODO: Refresh animators with operations
        }
    }
}
