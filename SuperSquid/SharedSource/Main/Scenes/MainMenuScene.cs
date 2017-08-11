#region Using Statements
using SuperSquid.Managers;
using System;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Components;
using WaveEngine.Components.Toolkit;
using WaveEngine.Components.Transitions;
using WaveEngine.Components.UI;
using WaveEngine.Framework;
using WaveEngine.Framework.Animation;
using WaveEngine.Framework.Graphics;
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

            play.Click += this.Play_Click;

            play.Entity.FindChild("ImageEntity").FindComponent<Transform2D>().Origin = Vector2.Center;
            play.Entity.AddComponent(new AnimationUI());
            EntityManager.Add(play);

            this.EntityManager.FindComponentFromEntityPath<TextComponent>("score.bestScoreText").Text = this.gameStorage.BestScore.ToString();
        }

#if ANDROID
        private async void Play_Click(object sender, EventArgs e)
        {
            var logIn = await WaveServices.GetService<SocialService>().Login();
            
            if (!logIn)
            {
                return;
            }
#else
        private void Play_Click(object sender, EventArgs e)
        {
#endif

            var gameContext = new ScreenContext("GamePlay", new GamePlayScene())
            {
                Behavior = ScreenContextBehaviors.DrawInBackground
            };

            WaveServices.ScreenContextManager.Pop();
            WaveServices.ScreenContextManager.Push(gameContext, new CrossFadeTransition(TimeSpan.FromSeconds(1.5f)));
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
            squidAppear = new SingleAnimation(this.VirtualScreenManager.VirtualWidth * 2, viewportWidthOverTwo, duration, EasingFunctions.Back);
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
