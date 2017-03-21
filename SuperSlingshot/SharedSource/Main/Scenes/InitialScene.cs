#region Using Statements
using SlingshotRampage.Services;
using SuperSlingshot.Components;
using SuperSlingshot.Managers;
using WaveEngine.Common.Input;
using WaveEngine.Framework;
using WaveEngine.Framework.Services;
using WaveEngine.Components.GameActions;
using System;
using WaveEngine.Framework.Graphics;
using WaveEngine.Common.Math;
#endregion

namespace SuperSlingshot.Scenes
{
    public class InitialScene : Scene
    {
        private NavigationManager navigationManager;

        private Entity startButton;
        private Entity title;
        private Transform2D startButtonTransform;
        private Transform2D titleTransform;

        protected override void CreateScene()
        {
            this.Load(WaveContent.Scenes.InitialScene);

            var audioService = WaveServices.GetService<AudioService>();
            audioService.Play(Audio.Music.backgroundmusic_mp3, 1.0f);

            this.startButton = this.EntityManager.Find(GameConstants.ENTITYSTARTBUTTON);
            this.startButtonTransform = startButton.FindComponent<Transform2D>();

            this.title = this.EntityManager.Find(GameConstants.ENTITYTITLE);
            this.titleTransform = title.FindComponent<Transform2D>();

            this.startButton.FindComponent<ButtonComponent>().StateChanged += this.StartStateChanged;

            this.navigationManager = WaveServices.GetService<NavigationManager>();
        }

        protected override void Start()
        {
            base.Start();

            
            this.CreateGameActionFromAction(() =>
                {
                    this.startButtonTransform.Scale = Vector2.One * 0.2f;
                    this.startButtonTransform.Opacity = 0.0f;

                    this.titleTransform.Scale = Vector2.One * 2.0f;
                    this.titleTransform.Opacity = 0.0f;

                })
                .Delay(TimeSpan.FromSeconds(1.0f))
                .ContinueWith(
                    new FloatAnimationGameAction(this.title, 0.0f, 1f, TimeSpan.FromSeconds(0.5f), EaseFunction.None, (f) =>
                    {
                        this.titleTransform.Opacity = f;
                    }),
                    new ScaleTo2DGameAction(this.title, Vector2.One, TimeSpan.FromSeconds(0.5f), EaseFunction.BounceOutEase, true)
                )
                .WaitAll()
                .Delay(TimeSpan.FromSeconds(0.5f))
                .ContinueWith(
                    new FloatAnimationGameAction(this.startButton, 0.0f, 1f, TimeSpan.FromSeconds(0.1f), EaseFunction.None, (f) =>
                    {
                        this.startButtonTransform.Opacity = f;
                    }),
                    new ScaleTo2DGameAction(this.startButton, Vector2.One * 0.8f, TimeSpan.FromSeconds(0.3f), EaseFunction.BackOutEase, true)
                )
                .WaitAll()
                .Run();
        }

        private void StartStateChanged(object sender, ButtonState currentState, ButtonState lastState)
        {
            if (currentState == ButtonState.Released && lastState == ButtonState.Pressed)
            {
                this.navigationManager.NavigateToLevelSelection();
            }
        }
    }
}
