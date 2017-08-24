using WaveEngine.Framework;

namespace P2PTank.Scenes
{
    public class IntroScene : Scene
    {
        protected override void CreateScene()
        {
            this.Load(WaveContent.Scenes.GamePlayScene);
        }
    }
}