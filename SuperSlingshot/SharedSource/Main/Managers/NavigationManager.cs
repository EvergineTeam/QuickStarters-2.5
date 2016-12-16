using System;
using SuperSlingshot.Scenes;
using WaveEngine.Common;
using WaveEngine.Framework.Services;

namespace SuperSlingshot.Managers
{
    public class NavigationManager : Service
    {
        public void NavigateToGameLevel(string level)
        {
            ScreenContext screenContext = new ScreenContext(
                "GameLevelContext",
                new GameScene(level));
            WaveServices.ScreenContextManager.Push(screenContext);
        }

        public void NavigateToScore(string level)
        {
            var gameScene = WaveServices.ScreenContextManager.FindContextByName("GameLevelContext").FindScene<GameScene>();

            if (gameScene == null)
            {
                return;
            }

            gameScene.Pause();

            ScreenContext screenContext = new ScreenContext(
                "ScoreContext",
                new ScoreScene(level))
            {
                Behavior = ScreenContextBehaviors.DrawInBackground
            };

            WaveServices.ScreenContextManager.To(screenContext);
        }

        public void NavigateBack(bool doDispose = false)
        {
            WaveServices.ScreenContextManager.Pop(doDispose);
        }

        public void NavigateToLevelSelection()
        {
            ScreenContext screenContext = new ScreenContext(
                "LevelSelectionContext",
                new LevelSelectionScene())
            {
                Behavior = ScreenContextBehaviors.DrawInBackground
            };

            WaveServices.ScreenContextManager.To(screenContext);
        }

        public void InitialNavigation()
        {
            ScreenContext screenContext = new ScreenContext(
                "InitialSceneContext",
                new GenericScene(WaveContent.Scenes.Backgrounds.Background1),
                new InitialScene())
            //new ScoreScene(1000, 3, 5))
            {
                Behavior = ScreenContextBehaviors.DrawInBackground | ScreenContextBehaviors.UpdateInBackground
            };

            WaveServices.ScreenContextManager.Push(screenContext);
        }

        public void ChangeState(bool pause)
        {
            var gameScene = WaveServices.ScreenContextManager.FindContextByName("GameLevelContext").FindScene<GameScene>();

            if (gameScene == null)
            {
                return;
            }

            ScreenContext screenContext = new ScreenContext(
                "MenuContext",
                new MenuScene());

            if (pause)
            {
                gameScene.Pause();

                WaveServices.ScreenContextManager.Push(screenContext);
            }
            else
            {
                gameScene.Resume();

                this.NavigateBack();
            }
        }
    }
}
