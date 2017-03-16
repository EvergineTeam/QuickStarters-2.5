using System;
using Match3.Components.Gameplay;
using Match3.Services;
using WaveEngine.Framework;
using Match3.Services.Navigation;
using WaveEngine.Components.GameActions;

namespace Match3.Scenes
{
    public class Gameplay : Scene
    {
        private GameLogic gameLogic;

        private GameboardAnimationsOrchestrator gameboardOrchestrator;

        protected override void CreateScene()
        {
            this.Load(WaveContent.Scenes.Gameplay);

            var gameboardEntity = this.EntityManager.Find("Panel.Content");
            this.gameboardOrchestrator = gameboardEntity.FindComponent<GameboardAnimationsOrchestrator>();

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
            this.gameLogic.GameFinished -= this.GameLogicGameFinished;

            this.CreateWaitConditionGameAction(() => !this.gameboardOrchestrator.IsAnimationInProgress)
                .ContinueWithAction(() => CustomServices.NavigationService.Navigate(NavigateCommands.DefaultForward))
                .Run();
        }
    }
}
