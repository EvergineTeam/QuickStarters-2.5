using Match3.Scenes;
using Match3.Services;
using WaveEngine.Common;
using WaveEngine.Framework.Services;

namespace Match3
{
    public class Game : WaveEngine.Framework.Game
    {
        public override void Initialize(IApplication application)
        {
            base.Initialize(application);

            CustomServices.NavigationService.StartNavigation();
        }
    }
}
