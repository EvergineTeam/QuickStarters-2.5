using System.Collections.Generic;
using SuperSlingshot.Scenes;
using WaveEngine.Common;
using WaveEngine.Framework.Services;

namespace SuperSlingshot.Managers
{
    public class NavigationManager : Service
    {
        private string[] LevelOrder =
{
            WaveContent.Scenes.Levels.Level1,
            WaveContent.Scenes.Levels.Level2,
            WaveContent.Scenes.Levels.Level3
        };

        public string CurrentLevel
        {
            get
            {
                return this.LevelOrder[this.CurrentLevelOrder];
            }
        }

        public int CurrentLevelOrder
        {
            get;
            private set;
        }

        public bool HasNextLevel
        {
            get
            {
                return this.CurrentLevelOrder < this.LevelOrder.Length - 1;
            }
        }

        public bool HasPrevLevel
        {
            get
            {
                return this.CurrentLevelOrder > 0;
            }
        }

        public void NavigateToNextLevel()
        {
            if (this.HasNextLevel)
            {
                this.NavigateToGameLevel(this.CurrentLevelOrder += 1);
            }
        }

        public void ReplayLevel()
        {
            this.NavigateToGameLevel(this.CurrentLevelOrder);
        }

        public void NavigateToGameLevel(int level)
        {
            this.CurrentLevelOrder = level;

            ScreenContext screenContext = new ScreenContext(
                "GameLevelContext",
                new GameScene(this.CurrentLevel));
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

            WaveServices.ScreenContextManager.CurrentContext.Behavior =
                ScreenContextBehaviors.DrawInBackground;

            ScreenContext screenContext = new ScreenContext(
                "ScoreContext",
                new ScoreScene(level));

            WaveServices.ScreenContextManager.Push(screenContext);
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
