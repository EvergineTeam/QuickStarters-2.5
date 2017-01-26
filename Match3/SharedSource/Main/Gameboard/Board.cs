using Match3.Gameboard;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Match3.Gameboard
{
    public class Board
    {
        private static Piece[] pieces = new Piece[]
        {
            new Piece { M = 1, N = 3 },
            new Piece { M = 1, N = 4, AddCandy = CandyTypes.FourInLine },
            new Piece { M = 3, N = 1 },
            new Piece { M = 4, N = 1, AddCandy = CandyTypes.FourInLine },
            new Piece { M = 2, N = 2, AddCandy = CandyTypes.FourInSquare }
        };

        private BoardGenerator generator;

        private Candy[][] currentStatus;

        public Candy[][] CurrentStatus
        {
            get { return this.currentStatus; }
        }
        
        public int SizeM
        {
            get;
            private set;
        }

        public int SizeN
        {
            get;
            private set;
        }

        public Board(int sizeM, int sizeN)
        {
            this.SizeM = sizeM;
            this.SizeN = sizeN;
            this.generator = new BoardGenerator();
            this.currentStatus = this.generator.Generate(sizeM, sizeN);
            this.ShuffleIfNecessary();
        }

        private bool ShuffleIfNecessary()
        {
            var hasBeenShuffled = false;
            while (!this.CurrentStatusIsValid())
            {
                this.generator.Shuffle(this.currentStatus);
                hasBeenShuffled = true;
            }

            return hasBeenShuffled;
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
            return result.Any();
        }

        public BoardOperation[] Move(Coordinate candyPosition, CandyMoves move)
        {
            var result = new List<BoardOperation>();

            if (this.UpdateCandiesPosition(candyPosition, move))
            {
                IEnumerable<BoardOperation> operations;
                while ((operations = this.GetCurrentOperations()).Any())
                {
                    result.AddRange(operations);
                    this.ExecuteOperation(operations);
                }

                if (result.Count == 0)
                {
                    this.UpdateCandiesPosition(candyPosition, this.ReverseMove(move));
                }
                else if (this.ShuffleIfNecessary())
                {
                    result.Add(new BoardOperation { Type = OperationTypes.Shuffle });
                }
            }

            return result.ToArray();
        }

        private bool MoveHasOperations(Coordinate candyPosition, CandyMoves move)
        {
            if (this.UpdateCandiesPosition(candyPosition, move))
            {
                var operation = this.GetCurrentOperations();
                this.UpdateCandiesPosition(candyPosition, this.ReverseMove(move));
                return operation.Any();
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

        private void ExecuteOperation(IEnumerable<BoardOperation> operations)
        {
            var boardStatus = this.currentStatus;
            foreach (var operation in operations)
            {
                foreach (var candyOperation in operation.CandyOperations)
                {
                    switch (operation.Type)
                    {
                        case OperationTypes.Remove:
                            boardStatus[candyOperation.PreviousPosition.X][candyOperation.PreviousPosition.Y].Type = CandyTypes.Empty;
                            break;
                        case OperationTypes.Add:
                            boardStatus[candyOperation.CurrentPosition.X][candyOperation.CurrentPosition.Y].Type = candyOperation.CandyProperties.Value.Type;
                            boardStatus[candyOperation.CurrentPosition.X][candyOperation.CurrentPosition.Y].Color = candyOperation.CandyProperties.Value.Color;
                            break;
                        case OperationTypes.Move:
                            boardStatus[candyOperation.CurrentPosition.X][candyOperation.CurrentPosition.Y].Type = boardStatus[candyOperation.PreviousPosition.X][candyOperation.PreviousPosition.Y].Type;
                            boardStatus[candyOperation.CurrentPosition.X][candyOperation.CurrentPosition.Y].Color = boardStatus[candyOperation.PreviousPosition.X][candyOperation.PreviousPosition.Y].Color;
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private IEnumerable<BoardOperation> GetCurrentOperations()
        {
            var resultOperations = new List<BoardOperation>();
            if (!this.TryGetAddOperation(ref resultOperations))
            {
                if (!this.TryGetMoveOperation(ref resultOperations))
                {
                    this.TryGetRemoveOperation(ref resultOperations);
                }
            }

            return resultOperations;
        }

        private bool TryGetAddOperation(ref List<BoardOperation> resultOperations)
        {
            var resultOperation = new BoardOperation();
            resultOperation.Type = OperationTypes.Add;
            var boardStatus = this.currentStatus;
            for (int j = 0; j < boardStatus[0].Length; j++)
            {
                for (int i = 0; i < boardStatus.Length; i++)
                {
                    if (boardStatus[i][j].Type == CandyTypes.Empty)
                    {
                        resultOperation.CandyOperations.Add(new CandyOperation
                        {
                            PreviousPosition = new Coordinate { X = -1, Y = -1 },
                            CurrentPosition = new Coordinate { X = i, Y = j },
                            CandyProperties = new Candy
                            {
                                Type = CandyTypes.Regular,
                                Color = this.generator.GetRandomCandyColor()
                            }
                        });
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if (resultOperation.CandyOperations.Count > 0)
            {
                resultOperations.Add(resultOperation);
                return true;
            }

            return false;
        }

        private bool TryGetMoveOperation(ref List<BoardOperation> resultOperations)
        {
            var resultOperation = new BoardOperation();
            resultOperation.Type = OperationTypes.Move;
            var boardStatus = this.currentStatus;
            for (int j = 0; j < boardStatus[0].Length; j++)
            {
                var empty = 0;
                for (int i = boardStatus.Length - 1; i >= 0; i--)
                {
                    if (boardStatus[i][j].Type == CandyTypes.Empty)
                    {
                        empty++;
                    }
                    else if (empty > 0)
                    {
                        resultOperation.CandyOperations.Add(new CandyOperation
                        {
                            PreviousPosition = new Coordinate { X = i, Y = j },
                            CurrentPosition = new Coordinate { X = i, Y = j - empty }
                        });
                    }
                }
            }

            if (resultOperation.CandyOperations.Count > 0)
            {
                resultOperations.Add(resultOperation);
                return true;
            }

            return false;
        }

        private bool TryGetRemoveOperation(ref List<BoardOperation> resultOperations)
        {
            var boardStatus = this.currentStatus;
            for (int p = 0; p < pieces.Length; p++)
            {
                var piece = pieces[p];
                for (int i = 0; i <= boardStatus.Length - piece.M; i++)
                {
                    for (int j = 0; j <= boardStatus[i].Length - piece.N; j++)
                    {
                        var matching = true;
                        var candyColor = boardStatus[i][j].Color;
                        for (int pi = 0; pi < piece.M && matching; pi++)
                        {
                            for (int pj = 0; pj < piece.N && matching; pj++)
                            {
                                if (boardStatus[i + pi][j + pj].Color != candyColor)
                                {
                                    matching = false;
                                }
                            }
                        }

                        if (matching)
                        {
                            var resultOperation = new BoardOperation();
                            resultOperation.Type = OperationTypes.Remove;
                            CandyColors color = CandyColors.Blue;
                            for (int pi = 0; pi < piece.M; pi++)
                            {
                                for (int pj = 0; pj < piece.N; pj++)
                                {
                                    var candy = boardStatus[i + pi][j + pj];
                                    color = candy.Color;
                                    if (candy.Type == CandyTypes.FourInLine)
                                    {
                                        // TODO remove line
                                    }
                                    if (candy.Type == CandyTypes.FourInSquare)
                                    {
                                        // TODO remove box of 3x3
                                    }

                                    resultOperation.CandyOperations.Add(new CandyOperation
                                    {
                                        PreviousPosition = new Coordinate { X = i + pi, Y = j + pj },
                                        CurrentPosition = new Coordinate { X = -1, Y = -1 }
                                    });
                                }
                            }

                            resultOperations.Add(resultOperation);

                            if (piece.AddCandy != null)
                            {
                                var addOperation = new BoardOperation();
                                addOperation.Type = OperationTypes.Add;
                                addOperation.CandyOperations.Add(new CandyOperation
                                {
                                    PreviousPosition = new Coordinate { X = -1, Y = -1 },
                                    CurrentPosition = new Coordinate { X = i, Y = j },
                                    CandyProperties = new Candy
                                    {
                                        Type = piece.AddCandy.Value,
                                        Color = color
                                    }
                                });

                                resultOperations.Add(addOperation);
                            }


                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public enum OperationTypes
        {
            Remove,
            Add,
            Move,
            Shuffle
        }

        public class CandyOperation
        {
            public Coordinate PreviousPosition { get; set; }

            public Coordinate CurrentPosition { get; set; }

            public Candy? CandyProperties { get; set; }
        }

        public class BoardOperation
        {
            public OperationTypes Type { get; set; }

            public List<CandyOperation> CandyOperations { get; set; }

            public BoardOperation()
            {
                this.CandyOperations = new List<CandyOperation>();
            }
        }

        private struct Piece
        {
            public int M;

            public int N;

            public CandyTypes? AddCandy { get; set; }
        }
    }
}
