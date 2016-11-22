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

            WaveServices.RegisterService(new ScoreService());
            WaveServices.RegisterService(new AnimationService());
            WaveServices.RegisterService(new AudioService());
            WaveServices.RegisterService(new GamePlayManager());

            //ScreenContext screenContext = new ScreenContext(new GameScene(WaveContent.Scenes.Levels.Level1));

            ScreenContext screenContext = new ScreenContext(
                //new GenericScene(WaveContent.Scenes.Backgrounds.Background1)
                new GameScene(WaveContent.Scenes.Levels.Level3)
                //new GenericScene(WaveContent.Scenes.Foregrounds.FrontScene)
                );

            //ScreenContext screenContext = new ScreenContext(new GameScene(WaveContent.Scenes.Levels.Level1));
            //ScreenContext screenContext = new ScreenContext(new GameScene(WaveContent.Scenes.Levels.TestLevel));
            WaveServices.ScreenContextManager.To(screenContext);
        }
    }
}
