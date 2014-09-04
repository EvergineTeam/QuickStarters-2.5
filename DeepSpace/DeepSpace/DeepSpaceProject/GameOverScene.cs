using DeepSpaceProject.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Components;
using WaveEngine.Components.Cameras;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Components.Transitions;
using WaveEngine.Components.UI;
using WaveEngine.Framework;
using WaveEngine.Framework.Animation;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
using WaveEngine.Framework.UI;

namespace DeepSpaceProject
{
    public class GameOverScene : Scene
    {
        private GameStorage gameStorage;
        private AnimationUI labelAnimation, playAnimation;
        private SingleAnimation playScaleAppear, playOpacityAppear;

        protected override void CreateScene()
        {
            FixedCamera2D camera2d = new FixedCamera2D("camera");
            camera2d.ClearFlags = ClearFlags.DepthAndStencil;
            EntityManager.Add(camera2d);
            
            Entity logo = new Entity()
            .AddComponent(new Transform2D() { X = WaveServices.ViewportManager.VirtualWidth / 2, Y = 300, Origin = new Vector2(0.5f, 0), DrawOrder = 0.1f })
            .AddComponent(new Sprite("Content/GameOver.wpk"))
            .AddComponent(new AnimationUI())
            .AddComponent(new SpriteRenderer(DefaultLayers.Alpha));

            this.gameStorage = Catalog.GetItem<GameStorage>();

            this.EntityManager.Add(logo);

            this.labelAnimation = logo.FindComponent<AnimationUI>();

            this.playScaleAppear = new SingleAnimation(0.2f, 1f, TimeSpan.FromSeconds(2), EasingFunctions.Back);
            this.playOpacityAppear = new SingleAnimation(0, 1, TimeSpan.FromSeconds(1), EasingFunctions.Cubic);

            var button = new Button()
            {
                Margin = new Thickness(0, 657, 0, 0),
                HorizontalAlignment = WaveEngine.Framework.UI.HorizontalAlignment.Center,
                VerticalAlignment = WaveEngine.Framework.UI.VerticalAlignment.Top,
                Text = string.Empty,
                IsBorder = false,
                BackgroundImage = "Content/restart.wpk",
                PressedBackgroundImage = "Content/restart.wpk"

            };

            button.Click += (o, e) =>
            {
                GamePlayScene scene = WaveServices.ScreenContextManager.FindContextByName("GamePlayContext").FindScene<GamePlayScene>();

                scene.State = Behaviors.GameState.Game;

                scene.Reset();

                WaveServices.ScreenContextManager.Pop(new CrossFadeTransition(TimeSpan.FromSeconds(0.5f)));
            };

            button.Entity.AddComponent(new AnimationUI());
            this.playAnimation = button.Entity.FindComponent<AnimationUI>();

            this.EntityManager.Add(button);

            var stackPanel = new StackPanel()
            {
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Bottom,
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 0, (WaveServices.ViewportManager.VirtualWidth - WaveServices.ViewportManager.RightEdge), 20)// (WaveServices.ViewportManager.VirtualWidth - WaveServices.ViewportManager.RightEdge), 20)
            };

            var bestScoreLabel = new Image("Content/BestScore.wpk")
            {
                VerticalAlignment = VerticalAlignment.Center
            };

            stackPanel.Add(bestScoreLabel);

            var bestScoreText = new TextBlock()
            {
                Text = this.gameStorage.BestScore.ToString(),
                FontPath = "Content/Space Age.wpk",
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(5,0,0,10)
            };
            stackPanel.Add(bestScoreText);

            this.EntityManager.Add(stackPanel);
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

            this.labelAnimation.BeginAnimation(Transform2D.XScaleProperty, this.playScaleAppear);
            this.labelAnimation.BeginAnimation(Transform2D.YScaleProperty, this.playScaleAppear);

            // Play button animation
            this.playAnimation.BeginAnimation(Transform2D.OpacityProperty, this.playOpacityAppear);
        }
    }
}
