#region Using Statements
using System;
using WaveEngine.Common;
using WaveEngine.Common.Graphics;
using WaveEngine.Framework;
using WaveEngine.Framework.Services;
#endregion

namespace DeepSpaceProject
{
    public class Game : WaveEngine.Framework.Game
    {
        public override void Initialize(IApplication application)
        {
            base.Initialize(application);

            WaveServices.ViewportManager.Activate(768, 1280, ViewportManager.StretchMode.UniformToFill);

            ScreenContext screenContext = new ScreenContext(new GamePlay());
            WaveServices.ScreenContextManager.To(screenContext);
        }
    }
}
