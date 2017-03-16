using System.Collections.Generic;

namespace SuperSlingshot.Managers
{
    public class GameStorage
    {
        public Dictionary<string, LevelScore> Scores { get; set; }

        public GameStorage()
        {
            this.Scores = new Dictionary<string, LevelScore>();
        }
    }
}
