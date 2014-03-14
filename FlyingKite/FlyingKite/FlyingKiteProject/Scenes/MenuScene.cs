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
    class MenuScene : Scene
    {
        private GameStorage gameStorage;

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

            this.gameStorage = Catalog.GetItem<GameStorage>();

            this.CreateUI();

#if DEBUG
            this.AddSceneBehavior(new DebugSceneBehavior(), SceneBehavior.Order.PreUpdate);
#endif
        }

        /// <summary>
        /// Creates the UI elements.
        /// </summary>
        private void CreateUI()
        {
            var logo = EntitiesFactory.CreateLogo();
            this.EntityManager.Add(logo);

            var button = EntitiesFactory.CreatePlayButton(404, 384);
            button.Click += (o, e) =>
            {
                WaveServices.ScreenContextManager.Pop();

                var scene = WaveServices.ScreenContextManager.FindContextByName("GameBackContext").FindScene<GameScene>();
                scene.SetState(GameScene.GameSceneStates.Gameplay);
            };
            this.EntityManager.Add(button);

            var bestScoreText = EntitiesFactory.CreateBestScore(620, this.gameStorage.BestScore);
            this.EntityManager.Add(bestScoreText);
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

            var gameScene = WaveServices.ScreenContextManager
                                        .FindContextByName("GameBackContext")
                                        .FindScene<GameScene>();

            gameScene.SetState(GameScene.GameSceneStates.Intro);
        }
    }
}
