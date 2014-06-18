#region File Description
//-----------------------------------------------------------------------------
// Super Squid
//
// Quickstarter for Wave University Tour 2014.
// Author: Wave Engine Team
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using SuperSquidProject.Commons;
using SuperSquidProject.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Components;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Components.UI;
using WaveEngine.Framework;
using WaveEngine.Framework.Animation;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
using WaveEngine.Framework.UI;
#endregion

namespace SuperSquidProject.Scenes
{
    public class GameOverScene : Scene
    {
        private GameStorage gameStorage;
        private GamePlayScene gameScene;

        private SingleAnimation scaleAppear, opacityAppear;
        private AnimationUI gameAnimation, overAnimation, restartAnimation;

        protected override void CreateScene()
        {
            // Allow transparent background
            this.RenderManager.ClearFlags = ClearFlags.DepthAndStencil;
            this.RenderManager.BackgroundColor = Color.Transparent;

            this.gameStorage = Catalog.GetItem<GameStorage>();
            this.gameScene = WaveServices.ScreenContextManager.FindContextByName("GamePlay")
                                                              .FindScene<GamePlayScene>();

            if (this.gameStorage.BestScore < this.gameScene.CurrentScore)
            {
                // Update best score
                this.gameStorage.BestScore = this.gameScene.CurrentScore;

                // Save storage game data
                GameStorage gameStorage = Catalog.GetItem<GameStorage>();
                WaveServices.Storage.Write<GameStorage>(gameStorage);
            }

            this.CreateUI();

            // Music Volume
            WaveServices.MusicPlayer.Volume = 0.2f;

            // Animations            
            Duration duration = TimeSpan.FromSeconds(1);          
            this.scaleAppear = new SingleAnimation(0.2f, 1f, TimeSpan.FromSeconds(2), EasingFunctions.Back);
            this.opacityAppear = new SingleAnimation(0, 1, duration, EasingFunctions.Cubic);
        }

        private void CreateUI()
        {
            // Dark background
            Entity dark = new Entity()
                                .AddComponent(new Transform2D()
                                {
                                    X = WaveServices.ViewportManager.LeftEdge,
                                    Y = WaveServices.ViewportManager.TopEdge,
                                    XScale = 1 / WaveServices.ViewportManager.RatioX,
                                    YScale = 1 / WaveServices.ViewportManager.RatioY,
                                    Opacity = 0.7f,
                                    DrawOrder = 0.9f,
                                })
                                .AddComponent(new ImageControl(Color.Black, (int)WaveServices.ViewportManager.ScreenWidth, (int)WaveServices.ViewportManager.ScreenHeight))
                                .AddComponent(new ImageControlRenderer(DefaultLayers.GUI));
            EntityManager.Add(dark);

            // Game
            Entity game = new Entity()
                                .AddComponent(new Transform2D()
                                {
                                    Origin = Vector2.Center,
                                    X = WaveServices.ViewportManager.VirtualWidth / 2,
                                    Y = 197,
                                })
                                .AddComponent(new AnimationUI())
                                .AddComponent(new Sprite(Directories.TexturePath + "game.wpk"))
                                .AddComponent(new SpriteRenderer(DefaultLayers.GUI));
            this.gameAnimation = game.FindComponent<AnimationUI>();
            EntityManager.Add(game);

            // Over
            Entity over = new Entity()
                                .AddComponent(new Transform2D()
                                {
                                    Origin = Vector2.Center,
                                    X = WaveServices.ViewportManager.VirtualWidth / 2,
                                    Y = 439,
                                })
                                .AddComponent(new AnimationUI())
                                .AddComponent(new Sprite(Directories.TexturePath + "over.wpk"))
                                .AddComponent(new SpriteRenderer(DefaultLayers.GUI));
            this.overAnimation = over.FindComponent<AnimationUI>();
            EntityManager.Add(over);

            // Restart Button
            Button restart = new Button()
            {
                Text = string.Empty,
                IsBorder = false,
                BackgroundImage = Directories.TexturePath + "restart.wpk",
                PressedBackgroundImage = Directories.TexturePath + "restartPressed.wpk",
                Margin = new Thickness(244, 571, 0, 0),
            };
            restart.Click += (s, o) =>
            {
                WaveServices.ScreenContextManager.FindContextByName("GamePlay").FindScene<GamePlayScene>().Reset();
                WaveServices.ScreenContextManager.Pop();
            };
            restart.Entity.FindChild("ImageEntity").FindComponent<Transform2D>().Origin = Vector2.Center;
            restart.Entity.AddComponent(new AnimationUI());
            this.restartAnimation = restart.Entity.FindComponent<AnimationUI>();
            EntityManager.Add(restart);

            // Last score text
            TextBlock lastScoresText = new TextBlock()
            {
                FontPath = Directories.FontsPath + "Bulky Pixels_16.wpk",
                Text = "your last score:",
                Foreground = new Color(223 / 255f, 244 / 255f, 255 / 255f),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new Thickness(161, 0, 0, 80),
            };
            EntityManager.Add(lastScoresText);

            // Last scores
            TextBlock lastScores = new TextBlock()
            {
                FontPath = Directories.FontsPath + "Bulky Pixels_26.wpk",
                Text = this.gameScene.CurrentScore.ToString(),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new Thickness(440, 0, 0, 90),
            };
            EntityManager.Add(lastScores);

            // StartFish
            Entity lastStarFish = new Entity()
                                    .AddComponent(new Transform2D()
                                    {
                                        Origin = Vector2.Center,
                                        X = 585,
                                        Y = 910,
                                        XScale = 0.7f,
                                        YScale = 0.7f,
                                    })
                                    .AddComponent(new Sprite(Directories.TexturePath + "starfish.wpk"))
                                    .AddComponent(new SpriteRenderer(DefaultLayers.GUI));
            EntityManager.Add(lastStarFish);

            // Best Scores text
            TextBlock bestScoresText = new TextBlock()
            {
                FontPath = Directories.FontsPath + "Bulky Pixels_16.wpk",
                Text = "your best score:",
                Foreground = new Color(223 / 255f, 244 / 255f, 255 / 255f),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new Thickness(161, 0, 0, 10),
            };
            EntityManager.Add(bestScoresText);

            // Best scores
            TextBlock bestScores = new TextBlock()
            {
                FontPath = Directories.FontsPath + "Bulky Pixels_26.wpk",
                Text = this.gameStorage.BestScore.ToString(),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new Thickness(440, 0, 0, 20),
            };
            EntityManager.Add(bestScores);

            // StartFish
            Entity bestStarFish = new Entity()
                                    .AddComponent(new Transform2D()
                                    {
                                        Origin = Vector2.Center,
                                        X = 585,
                                        Y = 980,
                                        XScale = 0.7f,
                                        YScale = 0.7f,
                                    })
                                    .AddComponent(new Sprite(Directories.TexturePath + "starfish.wpk"))
                                    .AddComponent(new SpriteRenderer(DefaultLayers.GUI));
            EntityManager.Add(bestStarFish);
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

            // Game animation
            this.gameAnimation.BeginAnimation(Transform2D.XScaleProperty, this.scaleAppear);
            this.gameAnimation.BeginAnimation(Transform2D.YScaleProperty, this.scaleAppear);
            this.gameAnimation.BeginAnimation(Transform2D.OpacityProperty, this.opacityAppear);

            // Over animation
            this.overAnimation.BeginAnimation(Transform2D.XScaleProperty, this.scaleAppear);
            this.overAnimation.BeginAnimation(Transform2D.YScaleProperty, this.scaleAppear);
            this.overAnimation.BeginAnimation(Transform2D.OpacityProperty, this.opacityAppear);

            // Restart button animation
            this.restartAnimation.BeginAnimation(Transform2D.XScaleProperty, this.scaleAppear);
            this.restartAnimation.BeginAnimation(Transform2D.YScaleProperty, this.scaleAppear);
            this.restartAnimation.BeginAnimation(Transform2D.OpacityProperty, this.opacityAppear);
        }
    }
}
