using System;
using System.Collections.Generic;
using System.Text;
using SuperSlingshot.Managers;
using WaveEngine.Common;
using WaveEngine.Components;
using WaveEngine.Framework.Services;

namespace SlingshotRampage.Services
{
    public class StorageService : Service
    {
        protected override void Initialize()
        {
            base.Initialize();

            var gameDataExits = WaveServices.Storage.Exists<GameStorage>();
            var gameData = gameDataExits ? WaveServices.Storage.Read<GameStorage>() : new GameStorage();
            Catalog.RegisterItem(gameData);
        }

        public LevelScore ReadScore(string level)
        {
            var catalog = Catalog.GetItem<GameStorage>();
            return catalog.Scores[level];
        }

        public Dictionary<string, LevelScore> ReadScores()
        {
            var catalog = Catalog.GetItem<GameStorage>();
            return catalog.Scores;
        }

        public bool WriteScore(LevelScore score, string level)
        {
            bool res = false;

            try
            {
                var catalog = Catalog.GetItem<GameStorage>();
                catalog.Scores[level] = score;

                WaveServices.Storage.Write(catalog);

                res = true;
            }
            catch (Exception)
            {
            }

            return res;
        }
    }
}
