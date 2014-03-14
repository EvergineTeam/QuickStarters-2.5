#region File Description
//-----------------------------------------------------------------------------
// OrbitRabbits
//
// Quickstarter for Wave University Tour 2014.
// Author: Wave Engine Team
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using OrbitRabbitsProject.Commons;
using OrbitRabbitsProject.Entities.Behaviors;
using OrbitRabbitsProject.Managers;
using System;
using WaveEngine.Common;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Components;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Components.Transitions;
using WaveEngine.Components.UI;
using WaveEngine.Framework;
using WaveEngine.Framework.Animation;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
using WaveEngine.Framework.UI;
#endregion

namespace OrbitRabbitsProject
{
    public class MainMenuScene : Scene
    {
        private SingleAnimation scaleUp, scaleDown;
        private AnimationUI playButtonAnimation;

        /// <summary>
        /// Creates the scene.
        /// </summary>
        /// <remarks>
        /// This method is called before all <see cref="T:WaveEngine.Framework.Entity" /> instances in this instance are initialized.
        /// </remarks>
        protected override void CreateScene()
        {
            RenderManager.BackgroundColor = Color.CornflowerBlue;

            //Background          
            Entity background = new Entity()
                            .AddComponent(new Transform2D()
                            {
                                ////X = WaveServices.ViewportManager.LeftEdge,
                                ////Y = WaveServices.ViewportManager.TopEdge,
                                ////XScale = (WaveServices.ViewportManager.ScreenWidth / 768) / WaveServices.ViewportManager.RatioX,
                                ////YScale = (WaveServices.ViewportManager.ScreenHeight / 1024) / WaveServices.ViewportManager.RatioY,
                                DrawOrder = 1,
                            })
                            .AddComponent(new Sprite(Directories.TexturePath + "background.wpk"))
                            .AddComponent(new SpriteRenderer(DefaultLayers.Opaque))
                            .AddComponent(new StretchBehavior());
            EntityManager.Add(background);

            // WaveLogo
            Entity waveLogo = new Entity()
                            .AddComponent(new Transform2D()
                            {
                                X = WaveServices.ViewportManager.VirtualWidth / 2,
                                Y = WaveServices.ViewportManager.TopEdge + 90,
                                Origin = Vector2.Center
                            })
                            .AddComponent(new SpriteAtlas(Directories.TexturePath + "game.wpk", "waveLogo"))
                            .AddComponent(new SpriteAtlasRenderer(DefaultLayers.Opaque));
            EntityManager.Add(waveLogo);

            // Logo
            Image logo = new Image(Directories.TexturePath + "largeLogo.wpk")
            {
                Margin = new Thickness(0, WaveServices.ViewportManager.TopEdge + 100, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Center,
            };
            EntityManager.Add(logo);

            // Moon Button
            Button moonButton = new Button()
            {                
                Text = string.Empty,
                IsBorder = false,
                Width = 631,
                Height = 639,
                BackgroundImage = Directories.TexturePath + "moonRelease.wpk",
                PressedBackgroundImage = Directories.TexturePath + "moonPressed.wpk",
                Margin = new Thickness(76, 425, 0, 0),
            };
            moonButton.Entity.AddComponent(new AnimationUI());
            this.playButtonAnimation = moonButton.Entity.FindComponent<AnimationUI>();
            moonButton.Entity.FindChild("ImageEntity").FindComponent<Transform2D>().Origin = Vector2.One / 2;
            moonButton.Click += (s, o) =>
            {
                SoundsManager.Instance.PlaySound(SoundsManager.SOUNDS.Click);
                SoundsManager.Instance.PlaySound(SoundsManager.SOUNDS.brokenGlass);
                WaveServices.ScreenContextManager.To(new ScreenContext(new GamePlayScene()), new SpinningSquaresTransition(TimeSpan.FromSeconds(1.5f)));
            };
            EntityManager.Add(moonButton);

            // Button animation
            float maxScale = 1.05f;
            float minScale = 0.95f;
            Duration duration = TimeSpan.FromSeconds(2.5f);
            this.scaleUp = new SingleAnimation(minScale, maxScale, duration);
            this.scaleUp.Completed += (s, o) =>
            {
                this.playButtonAnimation.BeginAnimation(Transform2D.XScaleProperty, this.scaleDown);
                this.playButtonAnimation.BeginAnimation(Transform2D.YScaleProperty, this.scaleDown);
            };
            this.scaleDown = new SingleAnimation(maxScale, minScale, duration);
            this.scaleDown.Completed += (s, o) =>
            {
                this.playButtonAnimation.BeginAnimation(Transform2D.XScaleProperty, this.scaleUp);
                this.playButtonAnimation.BeginAnimation(Transform2D.YScaleProperty, this.scaleUp);
            };

            // Play Text
            TextBlock playText = new TextBlock()
            {
                FontPath = Directories.FontsPath + "OCR A Std_20.wpk",
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
                FontPath = Directories.FontsPath + "OCR A Std_14.wpk",
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
                FontPath = Directories.FontsPath + "OCR A Std_20.wpk",
                Foreground = Color.White,
                Text = gameStorage.BestScore.ToString(),
                Margin = new Thickness(
                    250,
                    WaveServices.ViewportManager.BottomEdge - 45,
                    0,
                    0)
            };
            EntityManager.Add(maxScore);

            // Earth
            Entity earth = new Entity()
                            .AddComponent(new Transform2D()
                            {
                                X = 550,
                                Y = 954,
                            })
                            .AddComponent(new SpriteAtlas(Directories.TexturePath + "game.wpk", "earth"))
                            .AddComponent(new SpriteAtlasRenderer(DefaultLayers.GUI));
            EntityManager.Add(earth);
        }

        /// <summary>
        /// Allows to perform custom code when this instance is started.
        /// </summary>
        /// <remarks>
        /// This base method perfoms a layout pass.
        /// </remarks>
        protected override void Start()
        {
            base.Start();

            this.playButtonAnimation.BeginAnimation(Transform2D.XScaleProperty, this.scaleUp);
            this.playButtonAnimation.BeginAnimation(Transform2D.YScaleProperty, this.scaleUp);
        }
    }
}
