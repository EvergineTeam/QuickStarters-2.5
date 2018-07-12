#region Using Statements
using WaveEngine.Components.UI;
using WaveEngine.Framework;
using WaveEngine.Framework.Services;
using WaveFrogger.Services;
#endregion

namespace WaveFrogger.Scenes
{
    public class GameScene : Scene
    {

        protected override void CreateScene()
        {
            this.Load(WaveContent.Scenes.GameScene);
        }
    }
}
