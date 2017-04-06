using Match3.Factories;
using Match3.Gameboard;
using Match3.Helpers;
using Match3.Services;
using Match3.Services.Navigation;
using System;
using System.Collections.Generic;
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
        public const int DistanceBtwCandies = 150;

        [RequiredComponent]
        protected Transform2D transform;

        private GameLogic gameLogic;
        private int candyCounter;

        private List<CandyAttributesComponent> currentCandies;

        public event EventHandler<BoardOperation[]> OnBoardOperation;

        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();
            this.gameLogic = CustomServices.GameLogic;
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

        private void RegenerateGameboard()
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

            this.currentCandies = new List<CandyAttributesComponent>();
            this.candyCounter = 0;

            for (int coordX = 0; coordX < this.gameLogic.BoardSizeM; coordX++)
            {
                for (int coordY = 0; coordY < this.gameLogic.BoardSizeN; coordY++)
                {
                    var candyProperties = this.gameLogic.CurrentCandies[coordX][coordY];
                    var coord = new Coordinate() { X = coordX, Y = coordY };
                    this.AddCandyEntity(coord, candyProperties);
                }
            }
        }

        public CandyAttributesComponent AddCandyEntity(Coordinate coord, Candy properties)
        {
            Vector2 position = this.gameLogic.CalculateCandyPostion(coord);

            var candyEntity = GameplayFactory.CreateCandy(this.Owner.Scene, position, properties.Type, properties.Color);
            candyEntity.Name += "_" + this.candyCounter;
            var candyTouch = new CandyTouchComponent();
            var candyAttributes = new CandyAttributesComponent(coord, properties);
            candyTouch.OnMoveOperation += this.CandyTouch_OnMoveOperation;
            candyEntity.AddComponent(candyTouch)
                       .AddComponent(candyAttributes)
                       .AddComponent(new CandyAnimationBehavior());
            this.Owner.AddChild(candyEntity);

            this.candyCounter++;

            this.currentCandies.Add(candyAttributes);

            return candyAttributes;
        }

        public void RemoveCandyEntity(Coordinate coord)
        {
            var candyAttributes = this.FindCandyAttributes(coord);
            this.currentCandies.Remove(candyAttributes);
            this.Owner.RemoveChild(candyAttributes.Owner.Name);
        }

        public CandyAttributesComponent FindCandyAttributes(Coordinate coord)
        {
            return this.currentCandies.FirstOrDefault(c => c.Coordinate == coord);
        }

        public bool IsAnimating()
        {
            return this.currentCandies.Any(c => c.Animator.IsAnimating);
        }

        private void CandyTouch_OnMoveOperation(object sender, CandyMoves move)
        {
            var candyTouch = (CandyTouchComponent)sender;
            var candyAttributes = candyTouch.Owner.FindComponent<CandyAttributesComponent>();
            var coordinate = candyAttributes.Coordinate;
            
            var boardOperations = this.gameLogic.Move(coordinate, move);

            if (boardOperations.Any())
            {
                var destCoord = coordinate.Calculate(move);
                var destCandyAttribute = this.FindCandyAttributes(destCoord);
                destCandyAttribute.Coordinate = coordinate;
                candyAttributes.Coordinate = destCoord;
                destCandyAttribute.Animator.RefreshPositionAnimation();

                CustomServices.AudioPlayer.PlaySound(Services.Audio.Sounds.ValidMovement);
                this.OnBoardOperation?.Invoke(this, boardOperations);
            }
            else
            {
                CustomServices.AudioPlayer.PlaySound(Services.Audio.Sounds.InvalidMovement);
                candyAttributes.Animator.RefreshPositionAnimation();
            }
        }
    }
}
