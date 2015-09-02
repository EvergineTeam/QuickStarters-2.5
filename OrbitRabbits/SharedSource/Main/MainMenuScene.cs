using OrbitRabbits.Behaviors;
using OrbitRabbits.Commons;
using OrbitRabbits.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Components;
using WaveEngine.Components.Transitions;
using WaveEngine.Components.UI;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
using WaveEngine.Framework.UI;

namespace OrbitRabbits
{
    public class MainMenuScene : Scene
    {
        public Entity DetaultCamera2D
        {
            get
            {
                return this.EntityManager.Find("defaultCamera2D");
            }
        }

        protected override void CreateScene()
        {
            this.Load(@"Content/Scenes/MainMenuScene.wscene");

            this.DetaultCamera2D.FindComponent<Camera2D>().CenterScreen();

            this.CreateUI();
        }

        private void CreateUI()
        {
            // Moon Button
            Button moonButton = new Button()
            {
                Text = string.Empty,
                IsBorder = false,
                Width = 631,
                Height = 639,
                BackgroundImage = WaveContent.Assets.Textures.moonRelease_png,
                PressedBackgroundImage = WaveContent.Assets.Textures.moonRelease_png,
                Margin = new Thickness(76, 425, 0, 0),
            };

            moonButton.Entity.AddComponent(new ScaleCycleBehavior()
                {
                    MaxScale = 1.05f,
                    MinScale = 0.95f,
                    Period = 0.3f
                });
                        
            moonButton.Entity.FindChild("ImageEntity").FindComponent<Transform2D>().Origin = Vector2.Center;
            moonButton.Click += (s, o) =>
            {
                SoundsManager.Instance.PlaySound(SoundsManager.SOUNDS.Click);
                SoundsManager.Instance.PlaySound(SoundsManager.SOUNDS.brokenGlass);
                WaveServices.ScreenContextManager.To(new ScreenContext(new GamePlayScene()), new SpinningSquaresTransition(TimeSpan.FromSeconds(1.5f)));
            };
            EntityManager.Add(moonButton);

            // Play Text
            TextBlock playText = new TextBlock()
            {
                FontPath = WaveContent.Assets.Fonts.OCR_A_Extended_16_TTF,
                Foreground = new Color(119 / 255f, 250 / 255f, 255 / 255f),
                Text = "Play",
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(360, 885, 0, 0),
            };
            EntityManager.Add(playText);

            // Your Best Score text
            TextBlock bestScoreText = new TextBlock()
            {
                Width = 200,
                FontPath = WaveContent.Assets.Fonts.OCR_A_Extended_16_TTF,
                Foreground = new Color(119 / 255f, 250 / 255f, 255 / 255f),
                Text = "YOUR BEST SCORE:",
                Margin = new Thickness(
                    40,
                    WaveServices.ViewportManager.BottomEdge - 40,
                    0,
                    0),
            };
            EntityManager.Add(bestScoreText);


            // Scores   
            GameStorage gameStorage = Catalog.GetItem<GameStorage>();
            TextBlock maxScore = new TextBlock()
            {
                FontPath = WaveContent.Assets.Fonts.OCR_A_Extended_16_TTF,
                Foreground = Color.White,
                Text = gameStorage.BestScore.ToString(),
                Margin = new Thickness(
                    300,
                    WaveServices.ViewportManager.BottomEdge - 40,
                    0,
                    0)
            };
            EntityManager.Add(maxScore);
        }
    }
}
