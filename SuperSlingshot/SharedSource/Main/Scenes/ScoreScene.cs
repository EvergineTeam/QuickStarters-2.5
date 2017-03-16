#region Using Statements
using SlingshotRampage.Services;
using SuperSlingshot.Components;
using SuperSlingshot.Managers;
using WaveEngine.Common.Input;
using WaveEngine.Components.Toolkit;
using WaveEngine.Framework;
using WaveEngine.Framework.Services;
using WaveEngine.Components.GameActions;
using System;
#endregion

namespace SuperSlingshot.Scenes
{
    public class ScoreScene : Scene
    {
        private NavigationManager navigationManager;
        private string level;

        private LevelScore score;

        private ButtonComponent nextLevelButtonComponent;

        public ScoreScene(string level)
        {
            this.level = level;
        }

        protected override void CreateScene()
        {
            this.Load(WaveContent.Scenes.ScoreScene);

            this.navigationManager = WaveServices.GetService<NavigationManager>();

            var storageService = WaveServices.GetService<StorageService>();
            this.score = storageService.ReadScore(this.level);

            // Buttons
            var home = this.EntityManager.Find(GameConstants.ENTITYHOMEBUTTON);
            var menu = this.EntityManager.Find(GameConstants.ENTITYMENUBUTTON);
            var replay = this.EntityManager.Find(GameConstants.ENTITYREPLAYBUTTON);
            var next = this.EntityManager.Find(GameConstants.ENTITYNEXTBUTTON);

            home.FindComponent<ButtonComponent>().StateChanged += this.HomeStateChanged;
            menu.FindComponent<ButtonComponent>().StateChanged += this.MenuStateChanged;
            replay.FindComponent<ButtonComponent>().StateChanged += this.ReplayStateChanged;

            this.nextLevelButtonComponent = next.FindComponent<ButtonComponent>();
            this.nextLevelButtonComponent.StateChanged += this.NextStateChanged;
            this.nextLevelButtonComponent.IsBlocked = !this.navigationManager.HasNextLevel;
        }

        protected override void Start()
        {
            base.Start();

            // Score
            var score = this.EntityManager.Find(GameConstants.ENTITYSCORETEXT);
            var gems = this.EntityManager.Find(GameConstants.ENTITYGEMSTEXT);
            var scoreComponent = score.FindComponent<TextComponent>();
            var gemsComponent = gems.FindComponent<TextComponent>();

            this.CreateParallelGameActions(
                new FloatAnimationGameAction(score, 0.0f, this.score.Points, TimeSpan.FromSeconds(2f), EaseFunction.None, (f) =>
                {
                    scoreComponent.Text = Math.Round(f).ToString();
                }),
                new FloatAnimationGameAction(gems, 0.0f, this.score.Gems, TimeSpan.FromSeconds(2f), EaseFunction.None, (f) =>
                {
                    gemsComponent.Text = Math.Round(f).ToString();
                }))
                .WaitAll()
                .Run();

            // Rating
            var rating = this.EntityManager.Find(GameConstants.ENTITYRATINGSCORE);
            var starComponent = rating.FindComponent<StarScoreComponent>();
            starComponent.Score = this.score.StarScore;
        }

        private void HomeStateChanged(object sender, ButtonState currentState, ButtonState lastState)
        {
            if (currentState == ButtonState.Release && lastState == ButtonState.Pressed)
            {
                this.navigationManager.InitialNavigation();
            }
        }

        private void MenuStateChanged(object sender, ButtonState currentState, ButtonState lastState)
        {
            if (currentState == ButtonState.Release && lastState == ButtonState.Pressed)
            {
                this.navigationManager.NavigateToLevelSelection();
            }
        }

        private void ReplayStateChanged(object sender, ButtonState currentState, ButtonState lastState)
        {
            if (currentState == ButtonState.Release && lastState == ButtonState.Pressed)
            {
                this.navigationManager.ReplayLevel();
            }
        }

        private void NextStateChanged(object sender, ButtonState currentState, ButtonState lastState)
        {
            if (currentState == ButtonState.Release && lastState == ButtonState.Pressed)
            {
                this.navigationManager.NavigateToNextLevel();
            }
        }
    }
}
