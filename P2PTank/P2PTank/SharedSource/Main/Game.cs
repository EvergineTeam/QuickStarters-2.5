#region Using Statements
using WaveEngine.Common;
using WaveEngine.Framework.Services;
using P2PTank.Scenes;
#endregion

namespace P2PTank
{
    public class Game : WaveEngine.Framework.Game
    {
        public override void Initialize(IApplication application)
        {
            base.Initialize(application);

            // Better if serialized and stored 
            GameSettings.EnableFX = true;
            GameSettings.EnableMusic = true;
            GameSettings.FXVolume = 1.0f;
            GameSettings.MusicVolume = 1.0f;
            GameSettings.GamePadDeadZone = 0.25f;

            //ScreenContext screenContext = new ScreenContext(new P2PScene());
            //ScreenContext screenContext = new ScreenContext(new GamePlayScene());
            ScreenContext screenContext = new ScreenContext(new TestScene(WaveContent.Scenes.Levels.Level1));

            WaveServices.ScreenContextManager.To(screenContext);
            WaveServices.ScreenContextManager.SetDiagnosticsActive(true);
        }
    }
}
