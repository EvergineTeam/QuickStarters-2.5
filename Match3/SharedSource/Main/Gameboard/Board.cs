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

            if (this.UpdateCandiesPosition(candyPosition, move))
            {
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
            }

            return result.ToArray();
        }

        private bool MoveHasOperations(Coordinate candyPosition, CandyMoves move)
        {
            if (this.UpdateCandiesPosition(candyPosition, move))
            {
                var operations = this.GetCurrentOperations();
                this.UpdateCandiesPosition(candyPosition, this.ReverseMove(move));
                return operations.Length > 0;
            }

            return false;
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

        private bool UpdateCandiesPosition(Coordinate candyPosition, CandyMoves move)
        {
            if (this.ValidCoordinate(candyPosition))
            {
                var otherCandyPosition = candyPosition;
                switch (move)
                {
                    case CandyMoves.Left:
                        otherCandyPosition.X++;
                        break;
                    case CandyMoves.Right:
                        otherCandyPosition.X--;
                        break;
                    case CandyMoves.Top:
                        otherCandyPosition.Y--;
                        break;
                    case CandyMoves.Bottom:
                        otherCandyPosition.Y++;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("The indicated candy move is not valid.");
                }

                if (this.ValidCoordinate(otherCandyPosition))
                {
                    var otherCandy = this.currentStatus[otherCandyPosition.X][otherCandyPosition.Y];
                    this.currentStatus[otherCandyPosition.X][otherCandyPosition.Y] = this.currentStatus[candyPosition.X][candyPosition.Y];
                    this.currentStatus[candyPosition.X][candyPosition.Y] = otherCandy;

                    return true;
                }
            }

            return false;
        }

        private bool ValidCoordinate(Coordinate position)
        {
            return position.X >= 0 && position.Y >= 0 && position.X < this.currentStatus.Length && position.Y < this.currentStatus[0].Length;
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
