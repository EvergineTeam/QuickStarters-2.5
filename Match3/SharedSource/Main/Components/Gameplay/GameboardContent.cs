using Match3.Factories;
using Match3.Gameboard;
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
        private const int DistanceBtwItems = 150;

        [RequiredComponent]
        protected Transform2D transform;

        private bool isMoving;
        private GameLogic gameLogic;

        protected override void Initialize()
        {
            base.Initialize();
            this.gameLogic = CustomServices.GameLogic;
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

            var currentCandies = this.gameLogic.CurrentCandies;
            var mByTwo = (currentCandies.Length / 2f);
            var nByTwo = (currentCandies[0].Length / 2f);

            int indexI = 0;
            for (float i = -mByTwo + 0.5f; i < mByTwo + 0.5f; i++)
            {
                int indexJ = 0;
                for (float j = -nByTwo + 0.5f; j < nByTwo + 0.5f; j++)
                {
                    var coord = new Coordinate() { X = indexI, Y = indexJ };
                    var position = new Vector2(i * DistanceBtwItems, j * DistanceBtwItems);
                    var candy = currentCandies[indexI][indexJ];

                    //        var random = WaveServices.Random;
                    //        var allTypes = Enum.GetValues(typeof(CandyTypes));
                    //        var allColors = Enum.GetValues(typeof(CandyColors));
                    //        var randomType = (CandyTypes)allTypes.GetValue(random.Next(allTypes.Length));
                    //        var randomColor = (CandyColors)allColors.GetValue(random.Next(allColors.Length));

                    var candyEntity = GameplayFactory.CreateCandy(this.Owner.Scene, position, candy.Type, candy.Color);
                    candyEntity.Name += indexI + "_" + indexJ;
                    var candyTouch = new CandyTouchComponent() { Coordinate = coord };
                    candyTouch.OnMoveOperation += this.CandyTouch_OnMoveOperation;
                    candyEntity.AddComponent(candyTouch);
                    this.Owner.AddChild(candyEntity);
                    indexJ++;
                }
                indexI++;
            }
        }

        private async void CandyTouch_OnMoveOperation(object sender, CandyMoves move)
        {
            if (this.isMoving)
            {
                return;
            }

            this.isMoving = true;
            var candyTouch = (CandyTouchComponent)sender;
            foreach (var item in this.gameLogic.MoveIters(candyTouch.Coordinate, move))
            {
                this.RegenerateGameboard();
                await System.Threading.Tasks.Task.Delay(1000);
            }

            Debug.WriteLine("MOVE END!");
            this.isMoving = false;
        }
    }
}
