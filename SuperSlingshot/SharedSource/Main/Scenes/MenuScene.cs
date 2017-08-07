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
        private ButtonComponent resumeComponent;
        private ButtonComponent restartComponent;
        private ButtonComponent exitComponent;

        protected override void CreateScene()
        {
            this.Load(WaveContent.Scenes.MenuScene);

            this.gamePlayManager = WaveServices.GetService<GamePlayManager>();
            this.navigationManager = WaveServices.GetService<NavigationManager>();

            this.resumeComponent = this.EntityManager.Find(GameConstants.ENTITYMENURESUME).FindComponent<ButtonComponent>();
            this.restartComponent = this.EntityManager.Find(GameConstants.ENTITYMENURESTART).FindComponent<ButtonComponent>();
            this.exitComponent = this.EntityManager.Find(GameConstants.ENTITYMENUEXIT).FindComponent<ButtonComponent>();

            this.resumeComponent.StateChanged += this.ResumeButtonStateChanged;
            this.restartComponent.StateChanged += this.RestartButtonStateChanged;
            this.exitComponent.StateChanged += this.ExitButtonStateChanged;
        }

        protected override void Start()
        {
            base.Start();
        }

        protected override void End()
        {
            base.End();

            this.resumeComponent.StateChanged -= this.ResumeButtonStateChanged;
            this.restartComponent.StateChanged -= this.RestartButtonStateChanged;
            this.exitComponent.StateChanged -= this.ExitButtonStateChanged;
        }

        private void ResumeButtonStateChanged(object sender, ButtonState currentState, ButtonState lastState)
        {
            if (currentState == ButtonState.Released && lastState == ButtonState.Pressed)
            {
                this.gamePlayManager.ResumeGame();
            }
        }

        private void RestartButtonStateChanged(object sender, ButtonState currentState, ButtonState lastState)
        {
            if (currentState == ButtonState.Released && lastState == ButtonState.Pressed)
            {
                // TODO:
            }
        }

        private void ExitButtonStateChanged(object sender, ButtonState currentState, ButtonState lastState)
        {
            if (currentState == ButtonState.Released && lastState == ButtonState.Pressed)
            {
                this.navigationManager.InitialNavigation();
            }
        }
    }
}
