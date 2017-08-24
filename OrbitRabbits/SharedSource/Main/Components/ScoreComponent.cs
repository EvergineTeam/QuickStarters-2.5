using OrbitRabbits.Managers;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using WaveEngine.Components;
using WaveEngine.Components.Toolkit;
using WaveEngine.Framework;

namespace OrbitRabbits.Components
{
    [DataContract]
    public class ScoreComponent : Component
    {
        private int scores;

        private TextComponent scoreText;
        private TextComponent bestText;

        private GameStorage gameStorage;

        protected override void Initialize()
        {
            base.Initialize();

            this.gameStorage = Catalog.GetItem<GameStorage>();
            this.scoreText = this.Owner.FindChild("scoreText").FindComponent<TextComponent>();
            this.bestText = this.Owner.FindChild("bestText").FindComponent<TextComponent>();
        }

        /// <summary>
        /// Gets or sets the scores.
        /// </summary>
        public int Scores
        {
            get { return this.scores; }
            set
            {
                this.scores = value;
                this.scoreText.Text = this.scores.ToString();
                if (this.gameStorage.BestScore < this.scores)
                {
                    this.gameStorage.BestScore = this.scores;
                    this.bestText.Text = this.gameStorage.BestScore.ToString();
                }
            }
        }
    }
}
