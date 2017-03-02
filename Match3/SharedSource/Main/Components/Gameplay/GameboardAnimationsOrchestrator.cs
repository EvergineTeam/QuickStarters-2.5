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

        private Queue<BoardOperation> pendingOperations;

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

            this.pendingOperations = new Queue<BoardOperation>();
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

            this.EnqueueOperations(boardOperations);
        }

        private void EnqueueOperations(BoardOperation[] boardOperations)
        {
            foreach (var boardOp in boardOperations)
            {
                this.pendingOperations.Enqueue(boardOp);
            }
        }

        private void ProccessOperation(BoardOperation boardOp)
        {
            if (boardOp.Type == Board.OperationTypes.Remove)
            {
                foreach (var candyOp in boardOp.CandyOperations)
                {
                    this.gameboardContent.RemoveCandyEntity(candyOp.PreviousPosition);
                }
            }
            else if (boardOp.Type == Board.OperationTypes.Add)
            {
                foreach (var candyOp in boardOp.CandyOperations)
                {
                    this.gameboardContent.AddCandyEntity(candyOp.PreviousPosition, candyOp.CandyProperties.Value);
                }
            }

            if (boardOp.Type != OperationTypes.Remove)
            {
                foreach (var candyOp in boardOp.CandyOperations)
                {
                    var candyAttr = this.gameboardContent.FindCandyAttributes(candyOp.PreviousPosition);
                    candyAttr.Coordinate = candyOp.CurrentPosition;
                }
            }
        }

        protected override void Update(TimeSpan gameTime)
        {
            if (this.pendingOperations.Count > 0 &&
                !this.gameboardContent.IsAnimating())
            {
                var nextOperation = this.pendingOperations.Dequeue();
                this.ProccessOperation(nextOperation);
            }
        }
    }
}
