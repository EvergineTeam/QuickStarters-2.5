using WaveEngine.Framework;

namespace Match3.Scenes
{
    public class MenuBackground : Scene
    {
        protected override void CreateScene()
        {
            base.Load(WaveContent.Scenes.MenuBackground);
        }

        public override void Pause()
        {
            // This scene never should be paused
            //base.Pause();
        }
    }
}
