#region Using Statements
using OrbitRabbits.Commons;
using OrbitRabbits.Entities;
using System;
using WaveEngine.Common;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Components.Cameras;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Components.Graphics3D;
using WaveEngine.Components.UI;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Resources;
using WaveEngine.Framework.Services;
using WaveEngine.Framework.UI;
#endregion

namespace OrbitRabbits
{
    public class GamePlayScene : Scene
    {
        protected override void CreateScene()
        {
            this.Load(WaveContent.Scenes.GamePlayScene);  
            this.CreateUI();
        }

        private void CreateUI()
        {
            // Restart Button
            Button restart = new Button("RestartButton")
            {
                Text = string.Empty,
                IsBorder = false,
                BackgroundImage = WaveContent.Assets.Textures.restartRelease_png,
                PressedBackgroundImage = WaveContent.Assets.Textures.restartPressed_png,
                Margin = new Thickness(10, -120, 0, 0)
            };
            EntityManager.Add(restart);
        }
    }
}
