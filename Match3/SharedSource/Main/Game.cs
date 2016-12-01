using Match3.Scenes;
using WaveEngine.Common;
using WaveEngine.Framework.Services;

namespace Match3
{
    public class Game : WaveEngine.Framework.Game
    {
        public override void Initialize(IApplication application)
        {
            base.Initialize(application);

            ScreenContext screenContext = new ScreenContext(new MainMenu());
            WaveServices.ScreenContextManager.To(screenContext);
        }
    }
}
