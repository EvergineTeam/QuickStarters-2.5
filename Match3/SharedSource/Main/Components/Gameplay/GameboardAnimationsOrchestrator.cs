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
    public class GameboardAnimationsOrchestrator : Behavior
    {
        [RequiredComponent]
        protected GameboardContent gameboardContent;

        private Queue<Action> pendingOperations;

        [DontRenderProperty]
        public bool IsAnimationInProgress
        {
            get
            {
                return this.pendingOperations.Count > 0;
            }
        }

        protected override void DefaultValues()
        {
            base.DefaultValues();

            this.pendingOperations = new Queue<Action>();
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

            foreach (var boardOp in boardOperations)
            {
                this.ProccessOperation(boardOp);
            }
        }

        private void ProccessOperation(BoardOperation boardOp)
        {
            if (boardOp.Type == Board.OperationTypes.Remove)
            {
                this.pendingOperations.Enqueue(() =>
                {
                    foreach (var candyOp in boardOp.CandyOperations)
                    {
                        var candyAttr = this.gameboardContent.FindCandyAttributes(candyOp.PreviousPosition);
                        candyAttr.Animator.SetDisappearAnimation();
                    }
                });

                this.pendingOperations.Enqueue(() =>
                {
                    foreach (var candyOp in boardOp.CandyOperations)
                    {
                        this.gameboardContent.RemoveCandyEntity(candyOp.PreviousPosition);
                    }
                });
            }
            else if (boardOp.Type == Board.OperationTypes.Add)
            {
                this.pendingOperations.Enqueue(() =>
                {
                    foreach (var candyOp in boardOp.CandyOperations)
                    {
                        var candyAttr = this.gameboardContent.AddCandyEntity(candyOp.PreviousPosition, candyOp.CandyProperties.Value);

                        if (candyOp.PreviousPosition.Y >= 0)
                        {
                            candyAttr.Animator.SetAppearAnimation();
                        }
                    }
                });
            }

            if (boardOp.Type != OperationTypes.Remove)
            {
                this.pendingOperations.Enqueue(() =>
                {
                    foreach (var candyOp in boardOp.CandyOperations)
                    {
                        var candyAttr = this.gameboardContent.FindCandyAttributes(candyOp.PreviousPosition);
                        candyAttr.Coordinate = candyOp.CurrentPosition;
                        candyAttr.Animator.RefreshPositionAnimation();
                    }
                });
            }
        }

        protected override void Update(TimeSpan gameTime)
        {
            if (this.pendingOperations.Count > 0 &&
                !this.gameboardContent.IsAnimating())
            {
                var nextOperation = this.pendingOperations.Dequeue();
                nextOperation();
            }
        }
    }
}
