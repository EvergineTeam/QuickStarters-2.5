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
    public class MainMenuScene : Scene
    {
        private SingleAnimation superAppear, squidAppear, playScaleAppear, playOpacityAppear;
        private GameStorage gameStorage;

        protected override void CreateScene()
        {
            this.Load(WaveContent.Scenes.MainMenuScene);
            
            this.EntityManager.Find("defaultCamera2D").FindComponent<Camera2D>().CenterScreen();

            this.gameStorage = Catalog.GetItem<GameStorage>();

            this.CreateUI();
        }

        private void CreateUI()
        {
            // Play Button
            Button play = new Button("Play")
            {
                Text = string.Empty,
                IsBorder = false,
                BackgroundImage = WaveContent.Assets.Textures.play_png,
                PressedBackgroundImage = WaveContent.Assets.Textures.playPressed_png,
                Margin = new Thickness(244, 580, 0, 0),
            };

            play.Click += (s, o) =>
            {
                
                var gameContext = new ScreenContext("GamePlay", new GamePlayScene())
                {
                    Behavior = ScreenContextBehaviors.DrawInBackground
                };

                WaveServices.ScreenContextManager.Pop();
                WaveServices.ScreenContextManager.Push(gameContext, new CrossFadeTransition(TimeSpan.FromSeconds(1.5f)));
            };

            play.Entity.FindChild("ImageEntity").FindComponent<Transform2D>().Origin = Vector2.Center;
            play.Entity.AddComponent(new AnimationUI());            
            EntityManager.Add(play);

            // Best Scores
            TextBlock bestScores = new TextBlock("BestScores")
            {
                FontPath = WaveContent.Assets.Fonts.Bulky_Pixels_16_TTF,
                Text = "your best score:",
                Foreground = new Color(223 / 255f, 244 / 255f, 255 / 255f),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new Thickness(161, 0, 0, 30),
            };
            EntityManager.Add(bestScores);

            // Scores
            TextBlock scores = new TextBlock()
            {
                FontPath = WaveContent.Assets.Fonts.Bulky_Pixels_26_TTF,
                Text = this.gameStorage.BestScore.ToString(),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new Thickness(440, 0, 0, 40),
            };
            EntityManager.Add(scores);
        }

        protected override void Start()
        {
            base.Start();

            var superAnimation = this.EntityManager.Find("Super").FindComponent<AnimationUI>();
            var squidAnimation = this.EntityManager.Find("Squid").FindComponent<AnimationUI>();
            var playAnimation = this.EntityManager.Find<Button>("Play").Entity.FindComponent<AnimationUI>();


            // Animations
            float viewportWidthOverTwo = 768 / 2;
            Duration duration = TimeSpan.FromSeconds(1);
            superAppear = new SingleAnimation(-viewportWidthOverTwo, viewportWidthOverTwo, duration, EasingFunctions.Back);
            squidAppear = new SingleAnimation(WaveServices.ViewportManager.VirtualWidth * 2, viewportWidthOverTwo, duration, EasingFunctions.Back);
            playScaleAppear = new SingleAnimation(0.2f, 1f, TimeSpan.FromSeconds(2), EasingFunctions.Back);
            playOpacityAppear = new SingleAnimation(0, 1, duration, EasingFunctions.Cubic);

            // Super animation
            superAnimation.BeginAnimation(Transform2D.XProperty, superAppear);

            // Squid animation
            squidAnimation.BeginAnimation(Transform2D.XProperty, squidAppear);

            // Play button animation
            playAnimation.BeginAnimation(Transform2D.XScaleProperty, playScaleAppear);
            playAnimation.BeginAnimation(Transform2D.YScaleProperty, playScaleAppear);
            playAnimation.BeginAnimation(Transform2D.OpacityProperty, playOpacityAppear);
        }
    }
}
