using System;
using SlingshotRampage.Services;
using SuperSlingshot.Enums;
using SuperSlingshot.Scenes;
using WaveEngine.Common;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Services;
using WaveEngine.TiledMap;

namespace SuperSlingshot.Managers
{
    public class GamePlayManager : Service
    {
        private NavigationManager navigationManager;
        private float limitLeft, limitBottom, limitRight;

        public bool IsPaused { get; private set; }

        public override void OnActivated()
        {
            base.OnActivated();

            this.IsPaused = false;
        }

        public void InitGame()
        {
            this.IsPaused = false;

            var gameScene = WaveServices.ScreenContextManager.CurrentContext.FindScene<GameScene>();
            var tiledMap = gameScene.EntityManager.Find(GameConstants.ENTITYMAP).FindComponent<TiledMap>();
            var anchorLayer = tiledMap.ObjectLayers[GameConstants.LAYERANCHOR];
            var topLeftAnchor = anchorLayer.Objects.Find(o => o.Name == GameConstants.ANCHORTOPLEFT);
            var bottomRightAnchor = anchorLayer.Objects.Find(o => o.Name == GameConstants.ANCHORBOTTOMRIGHT);

            this.limitLeft = topLeftAnchor.X;
            this.limitBottom = bottomRightAnchor.Y;
            this.limitRight = bottomRightAnchor.X;

            var virtualScreenManager = gameScene.VirtualScreenManager;
            this.limitRight = virtualScreenManager.RightEdge;
        }

        public void PauseGame()
        {
            if (!this.IsPaused)
            {
                this.IsPaused = true;

                this.navigationManager = WaveServices.GetService<NavigationManager>();
                this.navigationManager.ChangeState(this.IsPaused);
            }
        }

        public void ResumeGame()
        {
            if (this.IsPaused)
            {
                this.IsPaused = false;

                this.navigationManager = WaveServices.GetService<NavigationManager>();
                this.navigationManager.ChangeState(this.IsPaused);
            }
        }

        private void FinishGame()
        {
            var gameScene = WaveServices.ScreenContextManager.CurrentContext.FindScene<GameScene>();

            // Calculate score
            var score = gameScene.Score;

            var maxPoint = (gameScene.NumBreakables * gameScene.BlockDestroyPoints) +
                (gameScene.GemPoints * gameScene.NumGems);

            score.StarScore = this.CalculateStarRate(score, maxPoint, gameScene.GemPoints);

            // store score
            var storageService = WaveServices.GetService<StorageService>();
            storageService.WriteScore(score, gameScene.Content);

            this.navigationManager = WaveServices.GetService<NavigationManager>();
            this.navigationManager.NavigateToScore(gameScene.Content);
        }

        private StarScoreEnum CalculateStarRate(LevelScore score, int maxPoints, int bonusPoints)
        {
            var points = score.Points;
            var bonus = score.Gems * bonusPoints;

            return (StarScoreEnum)Math.Round((double)(points + bonus) * 3 / maxPoints, 0);
        }

        public void NextBoulder()
        {
            if (!this.IsPaused)
            {
                var gameScene = WaveServices.ScreenContextManager.CurrentContext.FindScene<GameScene>();

                // TODO: C#6 
                // gameScene?.PrepareNextBoulder();
                if (gameScene != null)
                {
                    gameScene.PrepareNextBoulder();
                }
            }
        }

        public void RestartLevel()
        {
        }

        public void BoulderDead(Entity boulderEntity)
        {
            var gameScene = WaveServices.ScreenContextManager.CurrentContext.FindScene<GameScene>();

            if (gameScene != null)
            {
                if (gameScene.HasBreakables())
                {
                    if (gameScene.HasNextBouder)
                    {
                        gameScene.EntityManager.Remove(boulderEntity);
                        gameScene.PrepareNextBoulder();
                    }
                    else
                    {
                        // EndGame, show score and menu
                        this.FinishGame();
                    }
                }
                else
                {
                    this.FinishGame();
                }
            }
        }

        public bool CheckBounds(Vector2 position)
        {
            return !(MathHelper.FloatInRange(position.X, limitLeft, limitRight)
                    && MathHelper.FloatInRange(position.Y, limitBottom, float.MaxValue));
        }
    }
}

