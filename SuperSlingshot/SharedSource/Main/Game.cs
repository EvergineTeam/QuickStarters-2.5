#region Using Statements
using System;
using SlingshotRampage.Services;
using SuperSlingshot.Managers;
using SuperSlingshot.Scenes;
using WaveEngine.Common;
using WaveEngine.Common.Graphics;
using WaveEngine.Framework;
using WaveEngine.Framework.Services;
#endregion

namespace SuperSlingshot
{
    public class Game : WaveEngine.Framework.Game
    {
        public override void Initialize(IApplication application)
        {
            base.Initialize(application);

            var navigationManager = new NavigationManager();

            WaveServices.RegisterService(new ScoreService());
            WaveServices.RegisterService(new AnimationService());
            WaveServices.RegisterService(new AudioService());
            WaveServices.RegisterService(new GamePlayManager());
            WaveServices.RegisterService(navigationManager);

            ScreenContext screenContext = new ScreenContext(new LevelSelectionScene());

            navigationManager.NavigateToLevelSelection();
        }
    }
}
