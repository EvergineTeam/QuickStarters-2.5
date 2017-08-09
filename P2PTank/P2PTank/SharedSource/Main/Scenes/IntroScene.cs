using System;
using System.Collections.Generic;
using System.Text;
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
