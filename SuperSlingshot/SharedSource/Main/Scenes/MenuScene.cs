using SuperSlingshot.Components;
using SuperSlingshot.Managers;
using WaveEngine.Components.UI;
using WaveEngine.Framework;
using WaveEngine.Framework.Services;

namespace SuperSlingshot.Scenes
{
    public class MenuScene : Scene
    {
        private GamePlayManager gamePlayManager;

        protected override void CreateScene()
        {
            this.Load(WaveContent.Scenes.MenuScene);

            this.gamePlayManager = WaveServices.GetService<GamePlayManager>();

            var resumeButton = this.EntityManager.Find(GameConstants.ENTITYMENURESUME);
            var restartButton = this.EntityManager.Find(GameConstants.ENTITYMENURESTART);
            var exitButton = this.EntityManager.Find(GameConstants.ENTITYMENUEXIT);

            resumeButton.FindComponent<ButtonComponent>().StateChanged += this.ResumeButtonStateChanged;
            restartButton.FindComponent<ButtonComponent>().StateChanged += this.RestartButtonStateChanged;
            exitButton.FindComponent<ButtonComponent>().StateChanged += this.ExitButtonStateChanged;
        }

        private void ResumeButtonStateChanged(object sender, WaveEngine.Common.Input.ButtonState e)
        {
            this.gamePlayManager.ResumeGame();
        }

        private void RestartButtonStateChanged(object sender, WaveEngine.Common.Input.ButtonState e)
        {
        }
        private void ExitButtonStateChanged(object sender, WaveEngine.Common.Input.ButtonState e)
        {
        }

    }
}
