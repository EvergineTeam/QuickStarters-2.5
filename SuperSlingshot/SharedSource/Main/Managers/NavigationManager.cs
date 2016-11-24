using System;
using System.Collections.Generic;
using System.Text;
using SuperSlingshot.Scenes;
using WaveEngine.Common;
using WaveEngine.Framework.Services;

namespace SuperSlingshot.Managers
{
    public class NavigationManager : Service
    {
        public void NavigateToGameLevel(string level)
        {
            ScreenContext screenContext = new ScreenContext(new GameScene(level));
            WaveServices.ScreenContextManager.To(screenContext);
        }

        public void NavigateToLevelSelection()
        {
            ScreenContext screenContext = new ScreenContext(
                new GenericScene(WaveContent.Scenes.Backgrounds.Background1),
                new LevelSelectionScene());
            WaveServices.ScreenContextManager.To(screenContext);
        }
    }
}
