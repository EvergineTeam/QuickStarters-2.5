using SuperSlingshot.Components;
using SuperSlingshot.Managers;
using WaveEngine.Common.Input;
using WaveEngine.Framework;
using WaveEngine.Framework.Services;

namespace SuperSlingshot.Scenes
{
    public class MenuScene : Scene
    {
        private GamePlayManager gamePlayManager;
        private NavigationManager navigationManager;

        protected override void CreateScene()
        {
            this.Load(WaveContent.Scenes.MenuScene);

            this.gamePlayManager = WaveServices.GetService<GamePlayManager>();
            this.navigationManager = WaveServices.GetService<NavigationManager>();

            var resumeButton = this.EntityManager.Find(GameConstants.ENTITYMENURESUME);
            var restartButton = this.EntityManager.Find(GameConstants.ENTITYMENURESTART);
            var exitButton = this.EntityManager.Find(GameConstants.ENTITYMENUEXIT);

            resumeButton.FindComponent<ButtonComponent>().StateChanged += this.ResumeButtonStateChanged;
            restartButton.FindComponent<ButtonComponent>().StateChanged += this.RestartButtonStateChanged;
            exitButton.FindComponent<ButtonComponent>().StateChanged += this.ExitButtonStateChanged;
        }

        protected override void Start()
        {
            base.Start();
        }

        private void ResumeButtonStateChanged(object sender, ButtonState currentState, ButtonState lastState)
        {
            if (currentState == ButtonState.Release && lastState == ButtonState.Pressed)
            {
                this.gamePlayManager.ResumeGame();
            }
        }

        private void RestartButtonStateChanged(object sender, ButtonState currentState, ButtonState lastState)
        {
            if (currentState == ButtonState.Release && lastState == ButtonState.Pressed)
            {
                // TODO:
            }
        }

        private void ExitButtonStateChanged(object sender, ButtonState currentState, ButtonState lastState)
        {
            if (currentState == ButtonState.Release && lastState == ButtonState.Pressed)
            {
                this.navigationManager.InitialNavigation();
            }
        }
    }
}
