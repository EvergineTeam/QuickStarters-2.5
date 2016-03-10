using System;
using DeepSpace.Components.Appear;
using DeepSpace.Components.Gameplay;
using WaveEngine.Components.Transitions;
using WaveEngine.Components.UI;
using WaveEngine.Framework;
using WaveEngine.Framework.Animation;
using WaveEngine.Framework.Services;
using WaveEngine.Framework.UI;

namespace DeepSpace
{
    public class IntroScene : Scene
    {
        protected override void CreateScene()
        {
            this.Load(WaveContent.Scenes.IntroScene);
            this.CreateUI();
        }

        private void CreateUI()
        {
            var button = new Button()
            {
                Margin = new Thickness(0, 0, 0, 0
                ),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Text = string.Empty,
                IsBorder = false,
                BackgroundImage = WaveContent.Assets.PressStart_PNG,
                PressedBackgroundImage = WaveContent.Assets.PressStart_PNG
            };

            button.Entity.AddComponent(new AnimationUI());
            button.Entity.AddComponent(new OpacityApperComponent
            {
                From = 0.2f,
                DurationSeconds = 2
            });

            button.Click += this.OnStartButtonClicked;

            this.EntityManager.Add(button);
        }

        private void OnStartButtonClicked(object sender, EventArgs args)
        {
            var gameScene = WaveServices.ScreenContextManager.FindContextByName("GamePlayContext").FindScene<GamePlayScene>();

            gameScene.State = GameState.Game;

            WaveServices.ScreenContextManager.Pop(new CrossFadeTransition(TimeSpan.FromSeconds(0.5f)));
        }
    }
}
