#region Using Statements
using System;
using SuperSlingshot.Components;
using SuperSlingshot.Managers;
using WaveEngine.Common;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Input;
using WaveEngine.Common.Math;
using WaveEngine.Components.Cameras;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Components.Graphics3D;
using WaveEngine.Components.Toolkit;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Resources;
using WaveEngine.Framework.Services;
#endregion

namespace SuperSlingshot.Scenes
{
    public class ScoreScene : Scene
    {
        private NavigationManager navigationManager;

        private int points;
        private int bonus;
        private int rating;

        public ScoreScene(int points, int bonus, int rating)
        {
            this.points = points;
            this.bonus = bonus;
            this.rating = rating;
        }

        protected override void CreateScene()
        {
            this.Load(WaveContent.Scenes.ScoreScene);

            // Buttons
            var home = this.EntityManager.Find(GameConstants.ENTITYHOMEBUTTON);
            var menu = this.EntityManager.Find(GameConstants.ENTITYMENUBUTTON);
            var replay = this.EntityManager.Find(GameConstants.ENTITYREPLAYBUTTON);
            var next = this.EntityManager.Find(GameConstants.ENTITYNEXTBUTTON);

            home.FindComponent<ButtonComponent>().StateChanged += this.HomeStateChanged;
            menu.FindComponent<ButtonComponent>().StateChanged += this.MenuStateChanged;
            replay.FindComponent<ButtonComponent>().StateChanged += this.ReplayStateChanged;
            next.FindComponent<ButtonComponent>().StateChanged += this.NextStateChanged;

            this.navigationManager = WaveServices.GetService<NavigationManager>();
        }

        protected override void Start()
        {
            base.Start();

            // Score
            var score = this.EntityManager.Find(GameConstants.ENTITYSCORETEXT);
            var gems = this.EntityManager.Find(GameConstants.ENTITYGEMSTEXT);
            score.FindComponent<TextComponent>().Text = this.points.ToString();
            gems.FindComponent<TextComponent>().Text = this.bonus.ToString();

            // Rating
            var rating = this.EntityManager.Find(GameConstants.ENTITYRATINGSCORE);
            var res = MathHelper.Clamp(this.rating, 0, 3);
            var starComponent = rating.FindComponent<StarScoreComponent>();
            starComponent.Score = (Enums.StarScoreEnum)res;
        }

        private void HomeStateChanged(object sender, ButtonState currentState, ButtonState lastState)
        {
            if (currentState == ButtonState.Release && lastState == ButtonState.Pressed)
            {
            }
        }

        private void MenuStateChanged(object sender, ButtonState currentState, ButtonState lastState)
        {
            if (currentState == ButtonState.Release && lastState == ButtonState.Pressed)
            {
            }
        }

        private void ReplayStateChanged(object sender, ButtonState currentState, ButtonState lastState)
        {
            if (currentState == ButtonState.Release && lastState == ButtonState.Pressed)
            {
            }
        }

        private void NextStateChanged(object sender, ButtonState currentState, ButtonState lastState)
        {
            if (currentState == ButtonState.Release && lastState == ButtonState.Pressed)
            {
            }
        }
    }
}
