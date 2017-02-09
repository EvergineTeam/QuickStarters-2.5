using Match3.Gameboard;
using System;
using System.Collections.Generic;
using WaveEngine.Common;
using WaveEngine.Framework.Services;
using static Match3.Gameboard.Board;

namespace Match3.Services
{
    public class GameLogic : Service
    {
        public ulong CurrentScore { get; private set; }

        public Candy[][] CurrentCandies { get { return this.currentBoard.CurrentStatus; } }

        private Board currentBoard;

        private Timer currentTimer;

        public event EventHandler GameFinished;

        public void SelectLevel(int level)
        {
            //TODO Set level properties (max. score, time, etc.)
            this.currentBoard = new Board(6, 6);
        }

        public void Start()
        {
            this.currentTimer = WaveServices.TimerFactory.CreateTimer(TimeSpan.FromMinutes(1.5), this.EndGame);
        }

        private void EndGame()
        {
            WaveServices.TimerFactory.RemoveTimer(this.currentTimer);
            this.GameFinished?.Invoke(this, EventArgs.Empty);
        }

        public BoardOperation[] Move(Coordinate candyPosition, CandyMoves move)
        {
            var result = this.currentBoard.Move(candyPosition, move);
            this.SumScore(result);
            return result;
        }

        public IEnumerable<BoardOperation[]> MoveIters(Coordinate candyPosition, CandyMoves move)
        {
            var result = this.currentBoard.MoveIters(candyPosition, move);
            foreach (var item in result)
            {
                this.SumScore(item);
                yield return item;
            }
        }

        private void SumScore(BoardOperation[] operations)
        {
            foreach (var item in operations)
            {
                if(item.Type == OperationTypes.Remove)
                {
                    this.CurrentScore++;
                }
            }
        }

        public void Pause()
        {
            this.currentTimer.Pause();
        }

        public void Resume()
        {
            this.currentTimer.Resume();
        }
    }
}
