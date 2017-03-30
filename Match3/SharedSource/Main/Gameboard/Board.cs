using Match3.Gameboard;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Match3.Gameboard
{
    public class Board
    {
        private static Piece[] pieces = new Piece[]
        {
            new Piece { M = 2, N = 2, AddCandy = CandyTypes.FourInSquare },
            new Piece { M = 1, N = 4, AddCandy = CandyTypes.FourInLine },
            new Piece { M = 4, N = 1, AddCandy = CandyTypes.FourInLine },
            new Piece { M = 1, N = 3 },
            new Piece { M = 3, N = 1 }
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
                    this.UpdateCandiesPosition(candyPosition, move);
                }
                else if (this.ShuffleIfNecessary())
                {
                    result.Add(new BoardOperation { Type = OperationTypes.Shuffle });
                }
            }

            return result.ToArray();
        }

        public bool IsValidCoordinate(Coordinate position)
        {
            return position.X >= 0 && position.Y >= 0 && position.X < this.currentStatus.Length && position.Y < this.currentStatus[0].Length;
        }

        private bool MoveHasOperations(Coordinate candyPosition, CandyMoves move)
        {
            if (this.UpdateCandiesPosition(candyPosition, move))
            {
                var operation = this.GetCurrentOperations();
                this.UpdateCandiesPosition(candyPosition, move);
                return operation.Any();
            }

            return false;
        }

        private bool UpdateCandiesPosition(Coordinate candyPosition, CandyMoves move)
        {
            if (this.IsValidCoordinate(candyPosition))
            {
                var otherCandyPosition = candyPosition.Calculate(move);

                if (this.IsValidCoordinate(otherCandyPosition))
                {
                    var otherCandy = this.currentStatus[otherCandyPosition.X][otherCandyPosition.Y];
                    this.currentStatus[otherCandyPosition.X][otherCandyPosition.Y] = this.currentStatus[candyPosition.X][candyPosition.Y];
                    this.currentStatus[candyPosition.X][candyPosition.Y] = otherCandy;

                    return true;
                }
            }

            return false;
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
                            if (this.IsValidCoordinate(candyOperation.PreviousPosition))
                            {
                                boardStatus[candyOperation.PreviousPosition.X][candyOperation.PreviousPosition.Y].Type = CandyTypes.Empty;
                            }
                            break;
                        case OperationTypes.Add:
                            if (this.IsValidCoordinate(candyOperation.CurrentPosition))
                            {
                                boardStatus[candyOperation.CurrentPosition.X][candyOperation.CurrentPosition.Y].Type = candyOperation.CandyProperties.Value.Type;
                                boardStatus[candyOperation.CurrentPosition.X][candyOperation.CurrentPosition.Y].Color = candyOperation.CandyProperties.Value.Color;
                            }
                            break;
                        case OperationTypes.Move:
                            if (this.IsValidCoordinate(candyOperation.PreviousPosition) && this.IsValidCoordinate(candyOperation.CurrentPosition))
                            {
                                var current = boardStatus[candyOperation.CurrentPosition.X][candyOperation.CurrentPosition.Y];

                                boardStatus[candyOperation.CurrentPosition.X][candyOperation.CurrentPosition.Y] = boardStatus[candyOperation.PreviousPosition.X][candyOperation.PreviousPosition.Y];
                                boardStatus[candyOperation.PreviousPosition.X][candyOperation.PreviousPosition.Y] = current;
                            }
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
            for (int i = 0; i < boardStatus.Length; i++)
            {
                int yOffset = -1;
                for (int j = 0; j < boardStatus[0].Length; j++)
                {
                    if (boardStatus[i][j].Type == CandyTypes.Empty)
                    {
                        yOffset++;

                        resultOperation.AddCandyOperation(new CandyOperation
                        {
                            PreviousPosition = new Coordinate { X = i, Y = -1 },
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
                        var opCount = resultOperation.CandyOperations.Count;
                        while (yOffset > 0)
                        {
                            var coord = resultOperation.CandyOperations[opCount - yOffset].PreviousPosition;
                            coord.Y -= yOffset;
                            resultOperation.CandyOperations[opCount - yOffset].PreviousPosition = coord;

                            yOffset--;
                        }
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
            for (int i = 0; i < boardStatus.Length; i++)
            {
                var empty = 0;
                for (int j = boardStatus[0].Length - 1; j >= 0; j--)
                {
                    if (boardStatus[i][j].Type == CandyTypes.Empty)
                    {
                        empty++;
                    }
                    else if (empty > 0)
                    {
                        resultOperation.AddCandyOperation(new CandyOperation
                        {
                            PreviousPosition = new Coordinate { X = i, Y = j },
                            CurrentPosition = new Coordinate { X = i, Y = j + empty }
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
            var addOperations = new List<BoardOperation>();
            var removedCoordinate = new List<Coordinate>();
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
                                var currentCoordinate = new Coordinate() { X = i + pi, Y = j + pj };
                                if (boardStatus[i + pi][j + pj].Color != candyColor || removedCoordinate.Contains(currentCoordinate))
                                {
                                    matching = false;
                                }
                            }
                        }

                        if (matching)
                        {
                            var resultOperation = new BoardOperation();
                            resultOperation.Type = OperationTypes.Remove;
                            for (int pi = 0; pi < piece.M; pi++)
                            {
                                for (int pj = 0; pj < piece.N; pj++)
                                {
                                    this.CreateRemoveCandyOperation(removedCoordinate, boardStatus, piece, i + pi, j + pj, resultOperation);
                                }
                            }

                            resultOperations.Add(resultOperation);

                            if (piece.AddCandy != null)
                            {
                                var addOperation = new BoardOperation();
                                addOperation.Type = OperationTypes.Add;
                                addOperation.AddCandyOperation(new CandyOperation
                                {
                                    PreviousPosition = new Coordinate { X = i, Y = j },
                                    CurrentPosition = new Coordinate { X = i, Y = j },
                                    CandyProperties = new Candy
                                    {
                                        Type = piece.AddCandy.Value,
                                        Color = candyColor
                                    }
                                });

                                addOperations.Add(addOperation);
                            }
                        }
                    }
                }
            }

            resultOperations.AddRange(addOperations);

            return resultOperations.Any();
        }

        private void CreateRemoveCandyOperation(List<Coordinate> removedCoordinates, Candy[][] boardStatus, Piece piece, int i, int j, BoardOperation resultOperation)
        {
            var coordinate = new Coordinate { X = i, Y = j };
            if (removedCoordinates.Contains(coordinate) || !this.IsValidCoordinate(coordinate))
            {
                return;
            }

            removedCoordinates.Add(new Coordinate { X = i, Y = j });
            resultOperation.AddCandyOperation(new CandyOperation
            {
                PreviousPosition = new Coordinate { X = i, Y = j },
                CurrentPosition = new Coordinate { X = -1, Y = -1 }
            });

            var candy = boardStatus[i][j];
            if (candy.Type == CandyTypes.FourInLine)
            {
                // Remove line
                if (piece.M == 1)
                {
                    for (int fi = 0; fi < boardStatus.Length; fi++)
                    {
                        this.CreateRemoveCandyOperation(removedCoordinates, boardStatus, piece, fi, j, resultOperation);
                    }
                }
                else if (piece.N == 1)
                {
                    for (int fj = 0; fj < boardStatus[0].Length; fj++)
                    {
                        this.CreateRemoveCandyOperation(removedCoordinates, boardStatus, piece, i, fj, resultOperation);
                    }
                }
            }
            if (candy.Type == CandyTypes.FourInSquare)
            {
                // Remove box of 4x4
                for (int fi = -1; fi < 2; fi++)
                {
                    for (int fj = -1; fj < 2; fj++)
                    {
                        this.CreateRemoveCandyOperation(removedCoordinates, boardStatus, piece, i + fi, j + fj, resultOperation);
                    }
                }
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
