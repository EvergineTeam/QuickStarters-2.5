using DeepSpace.Components.Gameplay;
using DeepSpace.Managers;
using WaveEngine.Common.Graphics;
using WaveEngine.Components.UI;
using WaveEngine.Framework;
using WaveEngine.Framework.Services;
using WaveEngine.Framework.UI;

namespace DeepSpace
{
    public class GamePlayScene : Scene
    {
        private TextBlock scoreText;

        public GameState State
        {
            get
            {
                return this.EntityManager.Find("gameplay").FindComponent<GameplayBehavior>().State;
            }
            set
            {
                this.EntityManager.Find("gameplay").FindComponent<GameplayBehavior>().State = value;
            }
        }

        protected override void CreateScene()
        {
            this.Load(WaveContent.Scenes.GamePlayScene);

            this.scoreText = new TextBlock()
            {
                Text = "0",
                FontPath = WaveContent.Fonts.Space_Age_TTF,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new Thickness(80, 0, 0, 30),
                Foreground = new Color(117, 243, 237, 255)
            };

            this.EntityManager.Add(scoreText);
            
            WaveServices.GetService<ScoreManager>().CurrentScoreChanged += this.GamePlaySceneCurrentScoreChanged;
        }

        protected override void Start()
        {
            base.Start();
            this.Reset();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            WaveServices.GetService<ScoreManager>().CurrentScoreChanged -= this.GamePlaySceneCurrentScoreChanged;
        }

        private void GamePlaySceneCurrentScoreChanged(int score)
        {
            this.scoreText.Text = score.ToString();
        }

        public void Reset()
        {
            this.EntityManager.Find("gameplay").FindComponent<GameplayBehavior>().Reset();
        }
    }
}