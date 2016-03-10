using System;
using WaveEngine.Common;
using WaveEngine.Components;
using WaveEngine.Framework.Services;

namespace DeepSpace.Managers
{
    public class ScoreManager : Service
    {
        public event Action<int> CurrentScoreChanged;

        private int currentScore;

        public int CurrentScore
        {
            get { return this.currentScore; }
            set
            {
                var handler = this.CurrentScoreChanged;
                if (this.currentScore != value && handler != null)
                {
                    this.currentScore = value;
                    handler(this.currentScore);
                }
            }
        }

        protected override void Initialize()
        {
            this.CurrentScore = 0;

            var gameDataExists = WaveServices.Storage.Exists<GameStorage>();
            var gameStorage = gameDataExists ? WaveServices.Storage.Read<GameStorage>() : new GameStorage();
            Catalog.RegisterItem(gameStorage);
        }

        protected override void Terminate()
        {
        }
    }
}
