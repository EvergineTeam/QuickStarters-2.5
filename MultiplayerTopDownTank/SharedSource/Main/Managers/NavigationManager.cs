using MultiplayerTopDownTank.Scenes;
using WaveEngine.Common;
using WaveEngine.Framework;
using WaveEngine.Framework.Services;

namespace MultiplayerTopDownTank.Managers
{
    public class NavigationManager : Service
    {
        public void InitialNavigation()
        {
            ScreenContext screenContext = new ScreenContext(
                "MenuSceneContext",
                new MenuScene());

            WaveServices.ScreenContextManager.Push(screenContext);
        }

        public void NavigateToLobby()
        {
            WaveServices.ScreenContextManager
                .To(CreateScreenContext(
                    "LobbySceneContext",                                 
                    new LobbyScene(),
                    ScreenContextBehaviors.None));
        }

        public void NavigateToGame(int playerIndex)
        {
            WaveServices.ScreenContextManager
                .To(CreateScreenContext(
                    "GameSceneContext",
                    new GameScene(playerIndex),
                    ScreenContextBehaviors.None));
        }

        public void NavigateToScore()
        {
            WaveServices.ScreenContextManager
                .To(CreateScreenContext(
                    "ScoreSceneContext",
                    new ScoreScene(),
                    ScreenContextBehaviors.None));
        }

        public void NavigateBack(bool dispose = false)
        {
            WaveServices.ScreenContextManager.Pop(dispose);
        }

        private ScreenContext CreateScreenContext(
            string name, 
            Scene scene, 
            ScreenContextBehaviors behaviors = ScreenContextBehaviors.None)
        {
            return new ScreenContext(name, scene)
            {
                Behavior = behaviors
            };
        }
    }
}