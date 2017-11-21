#region Using Statements
using WaveEngine.Common;
using WaveEngine.Framework.Services;
using P2PTank.Scenes;
using P2PTank.Services;
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

            WaveServices.RegisterService(new AudioService());
            ScreenContext screenContext;

            if (WaveServices.Platform.PlatformFamily == PlatformFamily.Desktop)
            {
                screenContext = new ScreenContext(new GamePlayScene(WaveContent.Scenes.Levels.Level1));
            }
            else
            {
                screenContext = new ScreenContext(
                    new GamePlayScene(WaveContent.Scenes.Levels.Level1),
                    new VirtualJoystickScene())
                {
                    Behavior = ScreenContextBehaviors.UpdateInBackground | ScreenContextBehaviors.DrawInBackground
                };
            }

            WaveServices.ScreenContextManager.To(screenContext);
        }
    }
}
