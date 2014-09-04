using DeepSpaceProject.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
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
    public class IntroScene : Scene
    {

        private SingleAnimation playScaleAppear, playOpacityAppear;

        private AnimationUI labelAnimation, playAnimation;

        protected override void CreateScene()
        {
            FixedCamera2D camera2d = new FixedCamera2D("camera")
            {
                BackgroundColor = Color.Black,
                ClearFlags = ClearFlags.DepthAndStencil,
            };
            camera2d.BackgroundColor = Color.Black;
            EntityManager.Add(camera2d);

            Entity logo = new Entity()
            .AddComponent(new Transform2D() { X = WaveServices.ViewportManager.VirtualWidth / 2, Y = 227, Origin = Vector2.Center, DrawOrder = 0.1f })
            .AddComponent(new Sprite("Content/DeepSpace_logo.wpk"))
            .AddComponent(new AnimationUI())
            .AddComponent(new SpriteRenderer(DefaultLayers.Alpha));

            this.labelAnimation = logo.FindComponent<AnimationUI>();

            this.EntityManager.Add(logo);
           
            var button = new Button()
            {
                Margin = new Thickness(0, 657, 0, 0),
                HorizontalAlignment = WaveEngine.Framework.UI.HorizontalAlignment.Center,
                VerticalAlignment = WaveEngine.Framework.UI.VerticalAlignment.Top,
                Text = string.Empty,
                IsBorder = false,
                BackgroundImage = "Content/PressStart.wpk",
                PressedBackgroundImage = "Content/PressStart.wpk"

            };

            button.Click += (o, e) =>
            {
                var gameScene = WaveServices.ScreenContextManager.FindContextByName("GamePlayContext").FindScene<GamePlayScene>();

                gameScene.State = GameState.Game;

                WaveServices.ScreenContextManager.Pop(new CrossFadeTransition(TimeSpan.FromSeconds(0.5f)));
                ////WaveServices.ScreenContextManager.Push(gameContext, new CrossFadeTransition(TimeSpan.FromSeconds(1.5f)));
            };

            button.Entity.AddComponent(new AnimationUI());
            this.playAnimation = button.Entity.FindComponent<AnimationUI>();

            this.EntityManager.Add(button);


            this.playScaleAppear = new SingleAnimation(0.2f, 1f, TimeSpan.FromSeconds(2), EasingFunctions.Back);
            this.playOpacityAppear = new SingleAnimation(0, 1, TimeSpan.FromSeconds(1), EasingFunctions.Cubic);
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
