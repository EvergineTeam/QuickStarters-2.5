using System;
using Match3.Components.Gameplay;
using Match3.Gameboard;
using Match3.Services;
using WaveEngine.Framework;
using Match3.Services.Navigation;

namespace Match3.Scenes
{
    public class Gameplay : Scene
    {
        private GameLogic gameLogic;

        protected override void CreateScene()
        {
            this.Load(WaveContent.Scenes.Gameplay);

            this.gameLogic = CustomServices.GameLogic;
            this.gameLogic.GameFinished -= this.GameLogicGameFinished;
            this.gameLogic.GameFinished += this.GameLogicGameFinished;
        }

        protected override void Start()
        {
            base.Start();
            this.gameLogic.Start();
        }

        public override void Pause()
        {
            base.Pause();
            this.gameLogic.Pause();
        }

        public override void Resume()
        {
            base.Resume();
            this.gameLogic.Resume();
        }

        private void GameLogicGameFinished(object sender, EventArgs e)
        {
            CustomServices.NavigationService.Navigate(NavigateCommands.DefaultForward);
        }
    }
}
