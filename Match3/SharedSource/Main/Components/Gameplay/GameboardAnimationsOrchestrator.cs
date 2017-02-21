using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Match3.Gameboard;
using WaveEngine.Framework;
using static Match3.Gameboard.Board;
using WaveEngine.Components.GameActions;
using WaveEngine.Common.Attributes;

namespace Match3.Components.Gameplay
{
    [DataContract]
    public class GameboardAnimationsOrchestrator : Component
    {
        [RequiredComponent]
        protected GameboardContent gameboardContent;

        private List<GameAction> pendingAnimations;

        [DontRenderProperty]
        public bool IsAnimationInProgress
        {
            get
            {
                return this.pendingAnimations.Count > 0;
            }
        }

        protected override void DefaultValues()
        {
            base.DefaultValues();

            this.pendingAnimations = new List<GameAction>();
        }

        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            this.gameboardContent.OnBoardOperation += this.GameboardContent_OnBoardOperation;
        }

        private void GameboardContent_OnBoardOperation(object sender, BoardOperation[] boardOperations)
        {
            if (this.IsAnimationInProgress)
            {
                throw new InvalidOperationException("Previous operation must be finished before a new one start");
            }
            
            this.ProccessOperations(boardOperations);
        }

        private async void ProccessOperations(BoardOperation[] boardOperations)
        {
            foreach (var boardOp in boardOperations)
            {
                if (boardOp.Type == Board.OperationTypes.Remove)
                {
                    foreach (var candyOp in boardOp.CandyOperations)
                    {
                        this.gameboardContent.RemoveCandyEntity(candyOp.PreviousPosition);
                    }

                    await System.Threading.Tasks.Task.Delay(1000);
                }

                if (boardOp.Type == Board.OperationTypes.Add)
                {
                    foreach (var candyOp in boardOp.CandyOperations)
                    {
                        this.gameboardContent.AddCandyEntity(candyOp.PreviousPosition, candyOp.CandyProperties.Value);
                    }
                    await System.Threading.Tasks.Task.Delay(1000);
                }

                if (boardOp.Type != OperationTypes.Remove)
                {
                    foreach (var candyOp in boardOp.CandyOperations)
                    {
                        var candyAttr = this.gameboardContent.FindCandyAttributes(candyOp.PreviousPosition);
                        candyAttr.Coordinate = candyOp.CurrentPosition;
                    }
                    await System.Threading.Tasks.Task.Delay(1000);
                }
            }
        }
    }
}
