using Match3.Gameboard;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using WaveEngine.Common;
using WaveEngine.Framework.Services;
using static Match3.Gameboard.Board;

namespace Match3.Services
{
    public class GameLogic : Service
    {
        public int CurrentLevel { get; private set; }

        public ulong CurrentScore { get; private set; }

        public ulong[] StarsScores { get; private set; }

        public TimeSpan LeftTime { get { return this.currentTimer.Interval; } }

        public int BoardSizeM
        {
            get
            {
                return this.currentBoard.SizeM;
            }
        }

        public int BoardSizeN
        {
            get
            {
                return this.currentBoard.SizeN;
            }
        }

        public Candy[][] CurrentCandies { get { return this.currentBoard.CurrentStatus; } }

        private Board currentBoard;

        private GameState state;

        private Timer currentTimer;
        private TimeSpan levelTime;

        public event EventHandler GameFinished;

        public event EventHandler<ulong> ScoreUpdated;
        
        protected override void Initialize()
        {
            base.Initialize();
            this.LoadGameState();
        }

        public bool IsLevelUnlocked(int level)
        {
            return this.state.StarsByLevel.Count >= level - 1 && level <= 3;
        }

        public int StarsInLevel(int level)
        {
            return this.state.StarsByLevel.Count >= level ? this.state.StarsByLevel[level - 1] : 0;
        }

        public void SelectLevel(int level)
        {
            switch (level)
            {
                case 1:
                    this.levelTime = TimeSpan.FromMinutes(1.5);
                    this.StarsScores = new ulong[] { 60, 75, 100 };
                    break;
                case 2:
                    this.levelTime = TimeSpan.FromMinutes(2);
                    this.StarsScores = new ulong[] { 300, 475, 600 };
                    break;
                case 3:
                    this.levelTime = TimeSpan.FromMinutes(2.5);
                    this.StarsScores = new ulong[] { 500, 850, 2000 };
                    break;
                default:
                    throw new NotImplementedException();
            }

            this.CurrentLevel = level;
        }

        public void InitializeLevel()
        {
            this.CurrentScore = 0;
            this.currentBoard = new Board(6, 6);
        }

        public void Start()
        {
            this.currentTimer = WaveServices.TimerFactory.CreateTimer(this.levelTime, this.EndGame);
            this.currentTimer.Looped = false;
        }

        private void EndGame()
        {
            WaveServices.TimerFactory.RemoveTimer(this.currentTimer);
            this.SaveGameState();
            this.GameFinished?.Invoke(this, EventArgs.Empty);
        }

        public BoardOperation[] Move(Coordinate candyPosition, CandyMoves move)
        {
            if (this.LeftTime > TimeSpan.Zero)
            {
                var result = this.currentBoard.Move(candyPosition, move);
                this.SumScore(result);
                return result;
            }
            else
            {
                return new BoardOperation[0];
            }
        }

        public bool IsValidCoordinate(Coordinate candyPosition)
        {
            return this.currentBoard.IsValidCoordinate(candyPosition);
        }

        private void SumScore(BoardOperation[] operations)
        {
            foreach (var item in operations)
            {
                if (item.Type == OperationTypes.Remove)
                {
                    this.CurrentScore += (uint)item.CandyOperations.Count;
                }

                this.ScoreUpdated?.Invoke(this, this.CurrentScore);
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

        private void LoadGameState()
        {
            WaveServices.Storage.SetKnownTypes(new[] { typeof(GameState) });
            if (WaveServices.Storage.Exists("GameState"))
            {
                this.state = WaveServices.Storage.Read<GameState>("GameState");
            }
            else
            {
                this.state = new GameState
                {
                    StarsByLevel = new List<int>()
                };
            }
        }

        private void SaveGameState()
        {
            var starIndex = 0;
            for (int i = 0; i < this.StarsScores.Length; i++)
            {
                if (this.CurrentScore >= this.StarsScores[i])
                {
                    starIndex = i + 1;
                }
            }

            if (starIndex > 0 && this.StarsInLevel(this.CurrentLevel) < starIndex)
            {
                if (this.CurrentLevel - 1 >= this.state.StarsByLevel.Count)
                {
                    this.state.StarsByLevel.Add(starIndex);
                }
                else
                {
                    this.state.StarsByLevel.Insert(this.CurrentLevel - 1, starIndex);
                }
                WaveServices.Storage.Write(this.state);
            }
        }
    }

    [DataContract]
    public class GameState
    {
        [DataMember]
        public List<int> StarsByLevel { get; set; }
    }
}
