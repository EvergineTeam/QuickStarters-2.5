using WaveEngine.Framework;

namespace MultiplayerTopDownTank.Scenes
{
    public class LobbyScene : Scene
    {
        protected override void CreateScene()
        {
            this.Load(WaveContent.Scenes.LobbyScene);
        }
    }
}