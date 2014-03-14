#region File Description
//-----------------------------------------------------------------------------
// Flying Kite
//
// Quickstarter for Wave University Tour 2014.
// Author: Wave Engine Team
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using FlyingKiteProject.Behaviors;
using FlyingKiteProject.Drawables;
using FlyingKiteProject.Managers;
using FlyingKiteProject.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Components;
using WaveEngine.Components.Gestures;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Components.Transitions;
using WaveEngine.Components.UI;
using WaveEngine.Framework;
using WaveEngine.Framework.Animation;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Physics2D;
using WaveEngine.Framework.Services;
using WaveEngine.Framework.UI;
#endregion

namespace FlyingKiteProject.Scenes
{
    public class GameOverScene : Scene
    {
        private GameStorage gameStorage;

        private GameScene gameScene;

        /// <summary>
        /// Creates the scene.
        /// </summary>
        /// <remarks>
        /// This method is called before all <see cref="T:WaveEngine.Framework.Entity" /> instances in this instance are initialized.
        /// </remarks>
        protected override void CreateScene()
        {
            // Allow transparent background
            this.RenderManager.ClearFlags = ClearFlags.DepthAndStencil;
            this.RenderManager.BackgroundColor = Color.Transparent;

            this.gameStorage = Catalog.GetItem<GameStorage>();
            this.gameScene = WaveServices.ScreenContextManager.FindContextByName("GameBackContext")
                                                              .FindScene<GameScene>();

            if (this.gameStorage.BestScore < this.gameScene.CurrentScore)
            {
                // Update best score
                this.gameStorage.BestScore = this.gameScene.CurrentScore;

                // Save storage game data
                GameStorage gameStorage = Catalog.GetItem<GameStorage>();
                WaveServices.Storage.Write<GameStorage>(gameStorage);
            }

            this.CreateUI();

#if DEBUG
            this.AddSceneBehavior(new DebugSceneBehavior(), SceneBehavior.Order.PreUpdate);
#endif
        }

        /// <summary>
        /// Creates the UI elements and add them to the EntityManager.
        /// </summary>
        private void CreateUI()
        {
            var gameOver = EntitiesFactory.CreateGameOverText();
            this.EntityManager.Add(gameOver);

            var currentScore = EntitiesFactory.CreateCurrentScore(275);
            currentScore.Text = this.gameScene.CurrentScore.ToString();
            this.EntityManager.Add(currentScore);

            var bestScoreText = EntitiesFactory.CreateBestScore(395, this.gameStorage.BestScore);
            this.EntityManager.Add(bestScoreText);

            var button = EntitiesFactory.CreatePlayButton(423, 451);
            this.EntityManager.Add(button);

            button.Click += (o, e) =>
            {
                var screenTransition = new CoverTransition(TimeSpan.FromSeconds(0.25), CoverTransition.EffectOptions.FromBotton)
                {
                    EaseFunction = new CubicEase()
                    {
                        EasingMode = EasingMode.EaseInOut
                    }
                };

                WaveServices.ScreenContextManager.Pop(screenTransition);

                this.gameScene.SetState(GameScene.GameSceneStates.Gameplay);
                this.gameScene.Resume();
            };

            var background = EntitiesFactory.CreateGameOverBackground();
            this.EntityManager.Add(background);
        }
    }
}
