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
using WaveEngine.Common;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Common.Media;
using WaveEngine.Components;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Components.Transitions;
using WaveEngine.Components.UI;
using WaveEngine.Framework;
using WaveEngine.Framework.Animation;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
using WaveEngine.Framework.UI;
using WaveEngine.Materials;
#endregion

namespace SuperSquidProject.Scenes
{
    public class MainMenuScene : Scene
    {
        private GameStorage gameStorage;

        private SingleAnimation superAppear, squidAppear, playScaleAppear, playOpacityAppear;
        private AnimationUI superAnimation, squidAnimation, playAnimation;

        protected override void CreateScene()
        {
            // Allow transparent background
            this.RenderManager.ClearFlags = ClearFlags.DepthAndStencil;

            this.gameStorage = Catalog.GetItem<GameStorage>();

            this.CreateUI();

            // Animations
            float viewportWidthOverTwo = WaveServices.ViewportManager.VirtualWidth / 2;
            Duration duration = TimeSpan.FromSeconds(1);
            this.superAppear = new SingleAnimation(-viewportWidthOverTwo, viewportWidthOverTwo, duration, EasingFunctions.Back);
            this.squidAppear = new SingleAnimation(WaveServices.ViewportManager.VirtualWidth * 2, viewportWidthOverTwo, duration, EasingFunctions.Back);
            this.playScaleAppear = new SingleAnimation(0.2f, 1f, TimeSpan.FromSeconds(2), EasingFunctions.Back);
            this.playOpacityAppear = new SingleAnimation(0, 1, duration, EasingFunctions.Cubic);
        }

        private void CreateUI()
        {
            // Super
            Entity super = new Entity()
                                .AddComponent(new Transform2D()
                                {
                                    Origin = Vector2.Center,
                                    X = -WaveServices.ViewportManager.VirtualWidth / 2,
                                    Y = 220,
                                })
                                .AddComponent(new AnimationUI())
                                .AddComponent(new Sprite(Directories.TexturePath + "super.wpk"))
                                .AddComponent(new SpriteRenderer(DefaultLayers.GUI));
            this.superAnimation = super.FindComponent<AnimationUI>();
            EntityManager.Add(super);

            // Squid
            Entity squid = new Entity()
                                .AddComponent(new Transform2D()
                                {
                                    Origin = Vector2.Center,
                                    X = WaveServices.ViewportManager.VirtualWidth * 2,
                                    Y = 388,
                                })
                                .AddComponent(new AnimationUI())
                                .AddComponent(new Sprite(Directories.TexturePath + "squid.wpk"))
                                .AddComponent(new SpriteRenderer(DefaultLayers.GUI));
            this.squidAnimation = squid.FindComponent<AnimationUI>();
            EntityManager.Add(squid);

            // Play Button
            Button play = new Button()
            {
                Text = string.Empty,
                IsBorder = false,
                BackgroundImage = Directories.TexturePath + "play.wpk",
                PressedBackgroundImage = Directories.TexturePath + "playPressed.wpk",
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
            this.playAnimation = play.Entity.FindComponent<AnimationUI>();
            EntityManager.Add(play);

            // Best Scores
            TextBlock bestScores = new TextBlock()
            {
                FontPath = Directories.FontsPath + "Bulky Pixels_16.wpk",
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
                FontPath = Directories.FontsPath + "Bulky Pixels_26.wpk",
                Text = this.gameStorage.BestScore.ToString(),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new Thickness(440, 0, 0, 40),
            };
            EntityManager.Add(scores);

            // StartFish
            Entity starFish = new Entity()
                                    .AddComponent(new Transform2D()
                                    {
                                        Origin = Vector2.Center,
                                        X = 585,
                                        Y = 962,
                                    })
                                    .AddComponent(new Sprite(Directories.TexturePath + "starfish.wpk"))
                                    .AddComponent(new SpriteRenderer(DefaultLayers.GUI));
            EntityManager.Add(starFish);

            // Play background music
            WaveServices.MusicPlayer.Play(new MusicInfo(Directories.SoundsPath + "bg_music.mp3"));
            WaveServices.MusicPlayer.IsRepeat = true;
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

            // Super animation
            this.superAnimation.BeginAnimation(Transform2D.XProperty, this.superAppear);

            // Squid animation
            this.squidAnimation.BeginAnimation(Transform2D.XProperty, this.squidAppear);

            // Play button animation
            this.playAnimation.BeginAnimation(Transform2D.XScaleProperty, this.playScaleAppear);
            this.playAnimation.BeginAnimation(Transform2D.YScaleProperty, this.playScaleAppear);
            this.playAnimation.BeginAnimation(Transform2D.OpacityProperty, this.playOpacityAppear);
        }
    }
}
