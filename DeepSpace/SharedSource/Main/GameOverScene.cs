using System;
using DeepSpace.Components.Appear;
using DeepSpace.Components.Gameplay;
using DeepSpace.Managers;
using WaveEngine.Common.Graphics;
using WaveEngine.Components;
using WaveEngine.Components.Transitions;
using WaveEngine.Components.UI;
using WaveEngine.Framework;
using WaveEngine.Framework.Animation;
using WaveEngine.Framework.Services;
using WaveEngine.Framework.UI;

namespace DeepSpace
{
    public class GameOverScene : Scene
    {
        private TextBlock scoreText;

        protected override void CreateScene()
        {
            this.Load(WaveContent.Scenes.GameOverScene);

            var button = new Button()
            {
                Margin = new Thickness(0, 0, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Text = string.Empty,
                IsBorder = false,
                BackgroundImage = WaveContent.Assets.restart_png,
                PressedBackgroundImage = WaveContent.Assets.restart_png
            };

            button.Entity.AddComponent(new AnimationUI());
            button.Entity.AddComponent(new OpacityApperComponent
            {
                From = 0.2f,
                DurationSeconds = 2
            });

            button.Click += this.OnResetButtonClicked;

            this.EntityManager.Add(button);

            var gameStorage = Catalog.GetItem<GameStorage>();
            this.scoreText = new TextBlock()
            {
                Text = gameStorage.BestScore.ToString(),
                FontPath = WaveContent.Fonts.Space_Age_TTF,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new Thickness(0, 0, -10, 26),
                Foreground = new Color(117, 243, 237, 255)
            };

            this.EntityManager.Add(scoreText);
        }

        private void OnResetButtonClicked(object sender, EventArgs e)
        {
            GamePlayScene scene = WaveServices.ScreenContextManager.FindContextByName("GamePlayContext").FindScene<GamePlayScene>();
            scene.State = GameState.Game;
            scene.Reset();
            WaveServices.ScreenContextManager.Pop(new CrossFadeTransition(TimeSpan.FromSeconds(0.5f)));
        }
    }
}