#region Using Statements
using SuperSquid.Managers;
using System;
using System.Collections.Generic;
using WaveEngine.Common;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Components;
using WaveEngine.Components.Cameras;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Components.Graphics3D;
using WaveEngine.Components.Particles;
using WaveEngine.Components.Transitions;
using WaveEngine.Components.UI;
using WaveEngine.Framework;
using WaveEngine.Framework.Animation;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Resources;
using WaveEngine.Framework.Services;
using WaveEngine.Framework.UI;
#endregion

namespace SuperSquid.Scenes
{
    public class GameOverScene : Scene
    {
        private GameStorage gameStorage;
        private GamePlayScene gameScene;

        protected override void CreateScene()
        {
            this.Load(WaveContent.Scenes.GameOverScene);            
            this.EntityManager.Find("defaultCamera2D").FindComponent<Camera2D>().CenterScreen();

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
        }

        private void CreateUI()
        {                      
            // Play Button
            Button restart = new Button("Restart")
            {
                Text = string.Empty,
                IsBorder = false,
                BackgroundImage = WaveContent.Assets.Textures.restart_png,
                PressedBackgroundImage = WaveContent.Assets.Textures.restartPressed_png,
                Margin = new Thickness(244, 571, 0, 0),
            };

            restart.Click += (s, o) =>
            {
                WaveServices.ScreenContextManager.FindContextByName("GamePlay").FindScene<GamePlayScene>().Reset();
                WaveServices.ScreenContextManager.Pop();
            };

            restart.Entity.FindChild("ImageEntity").FindComponent<Transform2D>().Origin = Vector2.Center;
            restart.Entity.AddComponent(new AnimationUI());            
            EntityManager.Add(restart);

            // Last score text
            TextBlock lastScoresText = new TextBlock()
            {
                FontPath = WaveContent.Assets.Fonts.Bulky_Pixels_16_TTF,
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
                FontPath = WaveContent.Assets.Fonts.Bulky_Pixels_26_TTF,
                Text = this.gameScene.CurrentScore.ToString(),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new Thickness(440, 0, 0, 90),
            };
            EntityManager.Add(lastScores);

            // Best Scores text
            TextBlock bestScoresText = new TextBlock()
            {
                FontPath = WaveContent.Assets.Fonts.Bulky_Pixels_16_TTF,
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
                FontPath = WaveContent.Assets.Fonts.Bulky_Pixels_26_TTF,
                Text = this.gameStorage.BestScore.ToString(),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new Thickness(440, 0, 0, 20),
            };
            EntityManager.Add(bestScores);
        }

        protected override void Start()
        {
            base.Start(); 

            // Animations            
            Duration duration = TimeSpan.FromSeconds(1);
            var scaleAppear = new SingleAnimation(0.2f, 1f, TimeSpan.FromSeconds(2), EasingFunctions.Back);
            var opacityAppear = new SingleAnimation(0, 1, duration, EasingFunctions.Cubic);
            
            // Game animation
            var gameAnimation = this.EntityManager.Find("Game").FindComponent<AnimationUI>();
            gameAnimation.BeginAnimation(Transform2D.XScaleProperty, scaleAppear);
            gameAnimation.BeginAnimation(Transform2D.YScaleProperty, scaleAppear);
            gameAnimation.BeginAnimation(Transform2D.OpacityProperty, opacityAppear);

            // Over animation
            var overAnimation = this.EntityManager.Find("Over").FindComponent<AnimationUI>();
            overAnimation.BeginAnimation(Transform2D.XScaleProperty, scaleAppear);
            overAnimation.BeginAnimation(Transform2D.YScaleProperty, scaleAppear);
            overAnimation.BeginAnimation(Transform2D.OpacityProperty, opacityAppear);

            // Restart button animation
            var restartAnimation = this.EntityManager.Find("Restart").FindComponent<AnimationUI>();
            restartAnimation.BeginAnimation(Transform2D.XScaleProperty, scaleAppear);
            restartAnimation.BeginAnimation(Transform2D.YScaleProperty, scaleAppear);
            restartAnimation.BeginAnimation(Transform2D.OpacityProperty, opacityAppear);
        }
    }
}
