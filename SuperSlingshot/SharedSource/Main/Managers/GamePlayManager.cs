using SuperSlingshot.Scenes;
using WaveEngine.Common;
using WaveEngine.Framework;
using WaveEngine.Framework.Services;

namespace SuperSlingshot.Managers
{
    public class GamePlayManager : Service
    {
        private NavigationManager navigationManager;
        private Scene menuScene;

        public bool IsPaused { get; private set; }

        protected override void Initialize()
        {
            base.Initialize();
            this.menuScene = new MenuScene();
        }

        public override void OnActivated()
        {
            base.OnActivated();

            this.IsPaused = false;
        }


        public void InitGame()
        {
            this.IsPaused = false;
        }

        public void PauseGame()
        {
            if (!this.IsPaused)
            {
                this.IsPaused = true;

                this.navigationManager = WaveServices.GetService<NavigationManager>();
                this.navigationManager.ChangeState(this.IsPaused);
            }
        }

        public void ResumeGame()
        {
            if (this.IsPaused)
            {
                this.IsPaused = false;

                this.navigationManager = WaveServices.GetService<NavigationManager>();
                this.navigationManager.ChangeState(this.IsPaused);
            }
        }

        private void FinishGame()
        {
            // TODO
            this.PauseGame();
        }

        public void NextBoulder()
        {
            if (!this.IsPaused)
            {
                var gameScene = WaveServices.ScreenContextManager.CurrentContext.FindScene<GameScene>();

                // TODO: C#6 
                // gameScene?.PrepareNextBoulder();
                if (gameScene != null)
                {
                    gameScene.PrepareNextBoulder();
                }
            }
        }

        public void RestartLevel()
        {
        }

        public void BoulderDead(Entity boulderEntity)
        {
            var gameScene = WaveServices.ScreenContextManager.CurrentContext.FindScene<GameScene>();

            if (gameScene != null)
            {
                if(gameScene.HasBreakables())
                {
                    if (gameScene.HasNextBouder)
                    {
                        gameScene.EntityManager.Remove(boulderEntity);
                        gameScene.PrepareNextBoulder();
                    }
                    else
                    {
                        // EndGame, show score and menu
                        this.FinishGame();
                    }
                }
                else
                {
                    this.FinishGame();
                }
            }
        }
    }
}
