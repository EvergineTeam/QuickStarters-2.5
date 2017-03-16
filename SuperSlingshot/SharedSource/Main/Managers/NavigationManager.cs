using System.Collections.Generic;
using SuperSlingshot.Scenes;
using WaveEngine.Common;
using WaveEngine.Framework;
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

        private ScreenContext CreateScreenContext(string name, Scene scene, ScreenContextBehaviors behaviors = ScreenContextBehaviors.None)
        {
            return new ScreenContext(name, scene)
            {
                Behavior = behaviors
            };
        }

        public void NavigateToGameLevel(int level)
        {
            this.CurrentLevelOrder = level;

            WaveServices.ScreenContextManager.Push(
                this.CreateScreenContext("GameLevelContext",
                                         new GameScene(this.CurrentLevel)));
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

            WaveServices.ScreenContextManager.Push(
                this.CreateScreenContext("ScoreContext",
                                         new ScoreScene(level)));
        }

        public void NavigateBack(bool doDispose = false)
        {
            WaveServices.ScreenContextManager.Pop(doDispose);
        }

        public void NavigateToLevelSelection()
        {
            WaveServices.ScreenContextManager.To(this.CreateScreenContext("LevelSelectionContext",
                                                 new LevelSelectionScene(),
                                                 ScreenContextBehaviors.DrawInBackground));
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

            if (pause)
            {
                gameScene.Pause();
                WaveServices.ScreenContextManager.Push(this.CreateScreenContext("MenuContext", new MenuScene()));
            }
            else
            {
                gameScene.Resume();
                this.NavigateBack();
            }
        }
    }
}
