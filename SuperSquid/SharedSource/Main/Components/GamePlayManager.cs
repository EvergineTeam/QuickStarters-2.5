using SuperSquid.Commons;
using SuperSquid.Entities;
using SuperSquid.Entities.Behaviors;
using SuperSquid.Managers;
using SuperSquid.Scenes;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Media;
using WaveEngine.Components.Transitions;
using WaveEngine.Framework;
using WaveEngine.Framework.Services;

namespace SuperSquid.Components
{
    [DataContract(Namespace = "SuperSquid.Components")]
    public class GamePlayManager : Component
    {
        private ScorePanel scorePanel;
        private BackgroundScene backScene;

        private BlockBuilderBehavior blockBuilder;
        private SquidBehavior squid;

        /// <summary>
        /// Gets or sets the current score.
        /// </summary>
        /// <value>
        /// The current score.
        /// </value>
        public int CurrentScore
        {
            get
            {
                return this.scorePanel.Score;
            }

            set
            {
                this.scorePanel.Score = value;
            }
        }

        protected override void Initialize()
        {
            base.Initialize();

            this.scorePanel = this.EntityManager.Find<ScorePanel>("ScorePanel");

            //Backscene 
            var backContext = WaveServices.ScreenContextManager.FindContextByName("BackContext");
            if (backContext != null)
            {
                this.backScene = backContext.FindScene<BackgroundScene>();
            }

            this.blockBuilder = this.EntityManager.Find("BlockBuilder").FindComponent<BlockBuilderBehavior>();

            this.squid = this.EntityManager.Find("Squid").FindComponent<SquidBehavior>();
        }

        public void StartGame()
        {
            this.CurrentScore = 0;

            // Replay background music
            WaveServices.MusicPlayer.Volume = 1.0f;            

            //Resume back particles
            this.backScene.Resume();

            this.blockBuilder.Reset();
            this.squid.Appear();
                        
            this.blockBuilder.OnCollision += OnCollision;
            this.blockBuilder.OnStarCollected += OnStarCollected;
            this.blockBuilder.IsActive = true;
        }

        public void GameOver()
        {
            this.blockBuilder.IsActive = false;
            this.blockBuilder.OnCollision -= OnCollision;
            this.blockBuilder.OnStarCollected -= OnStarCollected;

            //Pause back particles
            this.backScene.Pause();


            var transition = new ColorFadeTransition(Color.White, TimeSpan.FromSeconds(0.5f));
            WaveServices.ScreenContextManager.Push(new ScreenContext(new GameOverScene()), transition);
        }

        private void OnStarCollected(object sender, EventArgs e)
        {
            this.CurrentScore++;
        }

        private void OnCollision(object sender, EventArgs e)
        {
            this.GameOver();
        }
    }
}
