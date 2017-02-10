using Match3.Services;
using Match3.Services.Navigation;
using System;
using WaveEngine.Framework;
using WaveEngine.Framework.Services;

namespace Match3.Scenes
{
    public class Loading : Scene
    {
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
            WaveServices.TimerFactory.CreateTimer(TimeSpan.FromSeconds(0.2), () =>
            {
                CustomServices.GameLogic.InitializeLevel();
                CustomServices.NavigationService.Navigate(NavigateCommands.DefaultForward);
            }, false, this);
        }
    }
}
