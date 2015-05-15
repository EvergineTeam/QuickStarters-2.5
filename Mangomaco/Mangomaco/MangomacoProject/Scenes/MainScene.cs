#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MangomacoProject.Factories;
using WaveEngine.Common.Math;
using WaveEngine.Components.Cameras;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
#endregion

namespace MangomacoProject.Scenes
{
    /// <summary>
    /// Main Scene
    /// </summary>
    public class MainScene : Scene
    {
        /// <summary>
        /// Creates the scene.
        /// </summary>
        /// <remarks>
        /// This method is called before all <see cref="T:WaveEngine.Framework.Entity" /> instances in this instance are initialized.
        /// </remarks>
        protected override void CreateScene()
        {
            // Camera
            var camera = new FixedCamera2D("Camera2D")
            {
                BackgroundColor = Resources.GameplayGBColor
            };
            EntityManager.Add(camera);

            // Background
            Entity background = new Entity()
                        .AddComponent(new Transform2D()
                        {
                            Origin = Vector2.Center,
                            X = WaveServices.ViewportManager.ScreenWidth / 2,
                            Y = WaveServices.ViewportManager.ScreenHeight / 2,
                            DrawOrder = 10,
                        })
                        .AddComponent(new Sprite(Resources.MenuBackground))
                        .AddComponent(new SpriteRenderer(DefaultLayers.Alpha));
            EntityManager.Add(background);

            // Play Button
            var playBtn = ControlsFactory.CreatePlayButton();
            EntityManager.Add(playBtn);
        }
    }
}
