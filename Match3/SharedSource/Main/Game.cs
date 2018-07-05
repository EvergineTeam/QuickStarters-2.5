using Match3.Services;
using Match3.Services.Audio;
using WaveEngine.Common;

namespace Match3
{
    public class Game : WaveEngine.Framework.Game
    {
        public override void Initialize(IApplication application)
        {
            base.Initialize(application);

            this.Load(WaveContent.GameInfo);

            CustomServices.AudioPlayer.PlayMusic(Songs.Menu);
            CustomServices.NavigationService.StartNavigation();
        }
    }
}
