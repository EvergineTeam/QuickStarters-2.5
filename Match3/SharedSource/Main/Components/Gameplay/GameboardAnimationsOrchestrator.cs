using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Match3.Gameboard;
using WaveEngine.Framework;
using WaveEngine.Common.Attributes;
using Match3.Services;
using static Match3.Gameboard.Board;

namespace Match3.Components.Gameplay
{
    [DataContract]
    public class GameboardAnimationsOrchestrator : Behavior
    {
        [RequiredComponent]
        protected GameboardContent gameboardContent;

        private GameLogic gameLogic;
        private ScoreComponent score;

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

            this.gameLogic = CustomServices.GameLogic;
            this.gameboardContent.OnBoardOperation += this.GameboardContent_OnBoardOperation;

            this.score = this.EntityManager.Find("ScorePanel").FindComponent<ScoreComponent>();
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
            this.pendingOperations.Enqueue(() =>
            {
                var operationScore = this.gameLogic.OperationScore(boardOp);
                this.score.CurrentScore += (float)operationScore;
            });

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
