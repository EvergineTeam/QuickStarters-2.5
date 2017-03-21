#region Using Statements
using System.Collections.Generic;
using SlingshotRampage.Services;
using SuperSlingshot.Components;
using SuperSlingshot.Managers;
using WaveEngine.Common.Input;
using WaveEngine.Framework;
using WaveEngine.Framework.Services;
#endregion

namespace SuperSlingshot.Scenes
{
    public class LevelSelectionScene : Scene
    {
        private NavigationManager navigationManager;
        private StorageService storageService;

        protected override void CreateScene()
        {
            this.Load(WaveContent.Scenes.LevelSelectionScene);

            this.navigationManager = WaveServices.GetService<NavigationManager>();
            this.storageService = WaveServices.GetService<StorageService>();
        }

        protected override void Start()
        {
            base.Start();

            var level1 = this.EntityManager.Find(GameConstants.ENTITYLEVEL1BUTTON);
            var level2 = this.EntityManager.Find(GameConstants.ENTITYLEVEL2BUTTON);
            var level3 = this.EntityManager.Find(GameConstants.ENTITYLEVEL3BUTTON);

            level1.FindComponent<ButtonComponent>().StateChanged += this.Level1StateChanged;
            level2.FindComponent<ButtonComponent>().StateChanged += this.Level2StateChanged;
            level3.FindComponent<ButtonComponent>().StateChanged += this.Level3StateChanged;

            var scores = storageService.ReadScores();

            this.UpdateLevelScore(level1.FindComponent<StarScoreComponent>(), WaveContent.Scenes.Levels.Level1, scores);
            this.UpdateLevelScore(level2.FindComponent<StarScoreComponent>(), WaveContent.Scenes.Levels.Level2, scores);
            this.UpdateLevelScore(level3.FindComponent<StarScoreComponent>(), WaveContent.Scenes.Levels.Level3, scores);
        }

        private void UpdateLevelScore(StarScoreComponent scoreComponent, string levelId, Dictionary<string, LevelScore> scores)
        {
            if (scores.ContainsKey(levelId))
            {
                scoreComponent.Score = scores[levelId].StarScore;
            }
        }

        private void Level1StateChanged(object sender, ButtonState currentState, ButtonState lastState)
        {
            if (currentState == ButtonState.Released && lastState == ButtonState.Pressed)
            {
                this.navigationManager.NavigateToGameLevel(0);
            }
        }

        private void Level2StateChanged(object sender, ButtonState currentState, ButtonState lastState)
        {
            if (currentState == ButtonState.Released && lastState == ButtonState.Pressed)
            {
                this.navigationManager.NavigateToGameLevel(1);
            }
        }

        private void Level3StateChanged(object sender, ButtonState currentState, ButtonState lastState)
        {
            if (currentState == ButtonState.Released && lastState == ButtonState.Pressed)
            {
                this.navigationManager.NavigateToGameLevel(2);
            }
        }
    }
}
