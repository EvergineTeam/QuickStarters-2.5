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
        public Entity DetaultCamera2D
        {
            get
            {
                return this.EntityManager.Find("defaultCamera2D");
            }
        }

        public Entity TapHand
        {
            get
            {
                return this.EntityManager.Find("TapHand");
            }
        }

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

            // Score Panel            
            var scorePanel = new ScorePanel("ScorePanel")
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(0, 0, -250, 0)
            };

            EntityManager.Add(scorePanel);
        }
    }
}
