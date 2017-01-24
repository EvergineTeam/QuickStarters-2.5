using System;
using System.Collections.Generic;

namespace Match3.Gameboard
{
    public class Board
    {
        private int[] pointsByStar;

        private BoardGenerator generator;

        private Candy[][] currentStatus;

        public Candy[][] CurrentStatus
        {
            get { return this.currentStatus; }
        }

        public Board(int sizeN, int sizeM, int[] pointsByStar)
        {
            this.pointsByStar = pointsByStar;
            this.generator = new BoardGenerator();
            this.currentStatus = this.generator.Generate(sizeN, sizeM);
            this.ShuffleIfNecessary();
        }

        private void ShuffleIfNecessary()
        {
            while (!this.CurrentStatusIsValid())
            {
                this.generator.Shuffle(this.currentStatus);
            }
        }

        private bool CurrentStatusIsValid()
        {
            return this.HasMovements() && !this.HasOperations();
        }

        private bool HasMovements()
        {
            var boardStatus = this.currentStatus;
            for (int i = 0; i < boardStatus.Length; i++)
            {
                for (int j = 0; j < boardStatus[i].Length; j++)
                {
                    var coordinate = new Coordinate { X = i, Y = j };
                    var result = this.MoveHasOperations(coordinate, CandyMoves.Bottom);
                    if (this.MoveHasOperations(coordinate, CandyMoves.Bottom) || this.MoveHasOperations(coordinate, CandyMoves.Right))
                    {
                        return true;
                    }

                }
            }

            return false;
        }

        private bool HasOperations()
        {
            var result = this.GetCurrentOperations();
            return result.Length > 0;
        }

        public object[] Move(Coordinate candyPosition, CandyMoves move)
        {
            var result = new List<object>();

            this.UpdateCandiesPosition(candyPosition, move);

            object[] operations;
            while ((operations = this.GetCurrentOperations()).Length > 0)
            {
                result.AddRange(operations);
                this.ExecuteOperations(operations);
            }

            if (result.Count == 0)
            {
                this.UpdateCandiesPosition(candyPosition, this.ReverseMove(move));
            }

            return result.ToArray();
        }

        private bool MoveHasOperations(Coordinate candyPosition, CandyMoves move)
        {
            this.UpdateCandiesPosition(candyPosition, move);
            var operations = this.GetCurrentOperations();
            this.UpdateCandiesPosition(candyPosition, this.ReverseMove(move));
            return operations.Length > 0;
        }

        private CandyMoves ReverseMove(CandyMoves move)
        {
            switch (move)
            {
                case CandyMoves.Left: return CandyMoves.Right;
                case CandyMoves.Right: return CandyMoves.Left;
                case CandyMoves.Top: return CandyMoves.Bottom;
                case CandyMoves.Bottom: return CandyMoves.Top;
                default:
                    throw new ArgumentOutOfRangeException("The indicated candy move is not valid.");
            }
        }

        private void UpdateCandiesPosition(Coordinate candyPosition, CandyMoves move)
        {
            var otherCandyXIndex = candyPosition.X;
            var otherCandyYIndex = candyPosition.Y;
            switch (move)
            {
                case CandyMoves.Left:
                    otherCandyXIndex++;
                    break;
                case CandyMoves.Right:
                    otherCandyXIndex--;
                    break;
                case CandyMoves.Top:
                    otherCandyYIndex--;
                    break;
                case CandyMoves.Bottom:
                    otherCandyYIndex++;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("The indicated candy move is not valid.");
            }

            var otherCandy = this.currentStatus[otherCandyXIndex][otherCandyYIndex];
            this.currentStatus[otherCandyXIndex][otherCandyYIndex] = this.currentStatus[candyPosition.X][candyPosition.Y];
            this.currentStatus[candyPosition.X][candyPosition.Y] = otherCandy;
        }

        private void ExecuteOperations(object[] operations)
        {
            // TODO
        }

        private object[] GetCurrentOperations()
        {
            // TODO
            return new object[0];
        }
    }
}
