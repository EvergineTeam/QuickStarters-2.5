using Match3.Services;
using Match3.Services.Navigation;
using WaveEngine.Components.GameActions;
using WaveEngine.Framework;

namespace Match3.Scenes
{
    public class Loading : Scene
    {
        private const double LoadingTime = 0.5;

        protected override void CreateScene()
        {
            this.Load(WaveContent.Scenes.Loading);
        }

        protected override void Start()
        {
            base.Start();
            this.LoadNextLevel();
        }

        public override void Resume()
        {
            base.Resume();
            this.LoadNextLevel();
        }

        private void LoadNextLevel()
        {
            this.CreateWaitConditionGameAction(() => !CustomServices.NavigationService.IsPerformingNavigation)
                .ContinueWithAction(() =>
                {
                    CustomServices.GameLogic.InitializeLevel();
                    CustomServices.NavigationService.Navigate(NavigateCommands.DefaultForward);
                })
                .Run();
        }
    }
}
