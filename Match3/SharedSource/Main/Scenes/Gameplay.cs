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
        
        protected override void CreateScene()
        {
            this.Load(WaveContent.Scenes.Gameplay);
            
            this.gameLogic = CustomServices.GameLogic;
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

            var gameboardEntity = this.EntityManager.Find("Panel.Content");
            var gameboardOrchestrator = gameboardEntity.FindComponent<GameboardAnimationsOrchestrator>();

            this.CreateWaitConditionGameAction(() => !gameboardOrchestrator.IsAnimationInProgress)
                .ContinueWithAction(() => CustomServices.NavigationService.Navigate(NavigateCommands.DefaultForward))
                .Run();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (this.gameLogic != null)
            {
                this.gameLogic.GameFinished -= this.GameLogicGameFinished;
            }
        }
    }
}
