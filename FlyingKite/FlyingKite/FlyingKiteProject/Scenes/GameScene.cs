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
using FlyingKiteProject.Entities;
using FlyingKiteProject.Layers;
using FlyingKiteProject.Resources;
using System;
using WaveEngine.Common;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
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
    public class GameScene : Scene
    {
        public enum GameSceneStates
        {
            Intro,
            Gameplay,
            Crash,
            GameOver
        }

        private const float DISTANCE_BWT_OBSTACLEPAIRS = 500;
        private const float OBSTACLEPAIRS_PULL_COUNT = 4;

        private int score;

        private Kite kite;

        private CrashEffect crashEffect;

        private TextBlock currentScoreTB;

        #region Properties
        public GameSceneStates CurrentState
        {
            get;
            private set;
        }

        public int CurrentScore
        {
            get
            {
                return this.score;
            }

            set
            {
                this.score = value;
                this.currentScoreTB.Text = this.score.ToString();
            }
        } 
        #endregion

        /// <summary>
        /// Creates the scene.
        /// </summary>
        /// <remarks>
        /// This method is called before all <see cref="T:WaveEngine.Framework.Entity" /> instances in this instance are initialized.
        /// </remarks>
        protected override void CreateScene()
        {
            Entity camera = new Entity()
                               .AddComponent(new Camera2D());

            EntityManager.Add(camera);

            this.RenderManager.RegisterLayerAfter(new ObstaclesLayer(this.RenderManager), DefaultLayers.Alpha);

            // Game Entities
            this.CreateBackground();
            this.CreateKite();
            this.CreateObstacles();

            this.crashEffect = EntitiesFactory.CreateCrashEffect();
            this.EntityManager.Add(this.crashEffect);

            // UI
            this.currentScoreTB = EntitiesFactory.CreateCurrentScore(40);
            this.EntityManager.Add(this.currentScoreTB);

#if DEBUG
            this.AddSceneBehavior(new DebugSceneBehavior(), SceneBehavior.Order.PreUpdate);
#endif
        }

        /// <summary>
        /// Creates the background.
        /// </summary>
        private void CreateBackground()
        {
            this.EntityManager.Add(EntitiesFactory.CreateBackground());
            this.EntityManager.Add(EntitiesFactory.CreateBackgroundCloud());
            this.EntityManager.Add(EntitiesFactory.CreateBackgroundPlane());

            float XOffset = 0;
            float screenWidthOver2 = WaveServices.ViewportManager.VirtualWidth / 4;
            for (int i = 0; i < 4; i++)
            {
                var initialX = XOffset + WaveServices.Random.Next((int)screenWidthOver2);
                XOffset += screenWidthOver2;

                this.EntityManager.Add(EntitiesFactory.CreateBackgroundKite(initialX));
            }
        }

        /// <summary>
        /// Creates the kite.
        /// </summary>
        private void CreateKite()
        {
            this.kite = EntitiesFactory.CreateKite();
            this.EntityManager.Add(this.kite);

            var ropeEnd = new Entity()
            .AddComponent(new Transform2D()
            {
                X = WaveServices.ViewportManager.VirtualWidth - 150,
                Y = WaveServices.ViewportManager.VirtualHeight
            });
            this.EntityManager.Add(ropeEnd);

            var kiteBall = EntitiesFactory.CreateKiteBall();
            kiteBall.AddComponent(new Follower2DBehavior(this.kite.Entity, Follower2DBehavior.FollowTypes.Y));

            this.EntityManager.Add(kiteBall);
            this.EntityManager.Add(EntitiesFactory.CreateLinkedRope(this.kite.Entity, new Vector2(0.96f, 0.21f), kiteBall, Vector2.Center));
            this.EntityManager.Add(EntitiesFactory.CreateLinkedRope(this.kite.Entity, new Vector2(0.92f, 0.66f), kiteBall, Vector2.Center));
            this.EntityManager.Add(EntitiesFactory.CreateLinkedRope(kiteBall, Vector2.Center, ropeEnd, this.kite.Transform.Position));
        }

        /// <summary>
        /// Creates the obstacles pairs.
        /// </summary>
        private void CreateObstacles()
        {
            for (int i = 0; i < 4; i++)
            {
                var obstacle = EntitiesFactory.CreateObstaclePair(
                    OBSTACLEPAIRS_PULL_COUNT * DISTANCE_BWT_OBSTACLEPAIRS);

                this.EntityManager.Add(obstacle);
            }
        }

        /// <summary>
        /// Resets the scene. Used to restart the game.
        /// </summary>
        /// <param name="setNewKite">if set to <c>true</c> [set new kite].</param>
        private void ResetScene(bool setNewKite)
        {
            int obstacleIndex = 0;
            foreach (var obj in this.EntityManager.FindAllByTag("OBSTACLE"))
            {
                var obstaclePair = (ObstaclePair)obj;

                obstaclePair.Transform2D.X =
            WaveServices.ViewportManager.RightEdge + DISTANCE_BWT_OBSTACLEPAIRS * obstacleIndex;

                obstaclePair.Entity.Enabled = true;
                obstaclePair.StarAvaible = true;

                obstacleIndex++;
            }

            if (setNewKite)
            {
                this.kite.SetNewColor();
            }
        }

        /// <summary>
        /// Enable or disable the ScrollBehavior of all the entities inside the EntityManger of the scene.
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        private void SetScrollEnable(bool value)
        {
            foreach (var entity in this.EntityManager.EntityGraph)
            {
                var scrollBehavior = entity.FindComponent<ScrollBehavior>();

                if (scrollBehavior != null)
                {
                    scrollBehavior.IsActive = value;
                }
            }
        }

        /// <summary>
        /// Changes the scene state.
        /// </summary>
        /// <param name="state">The state.</param>
        public void SetState(GameSceneStates state)
        {
            var previusState = this.CurrentState;
            this.CurrentState = state;

            switch (state)
            {
                case GameSceneStates.Intro:
                    this.currentScoreTB.IsVisible = false;

                    foreach (var obstacle in this.EntityManager.FindAllByTag("OBSTACLE"))
                    {
                        var obstaclePair = (ObstaclePair)obstacle;
                        obstaclePair.Entity.Enabled = false;
                    }

                    this.EntityManager.Find("kite")
                                      .FindComponent<KiteBehavior>()
                                      .SetState(KiteBehavior.KiteStates.TakeOff);
                    break;

                case GameSceneStates.Gameplay:
                    this.currentScoreTB.IsVisible = true;
                    this.CurrentScore = 0;
                    this.EntityManager.Find("kite")
                                      .FindComponent<KiteBehavior>()
                                      .SetState(KiteBehavior.KiteStates.Gameplay);

                    this.ResetScene(previusState != GameSceneStates.Intro);
                    this.SetScrollEnable(true);
                    break;

                case GameSceneStates.Crash:
                    // Crash Effect
                    this.crashEffect.DoEffect();

                    //Dissable Scroll
                    this.SetScrollEnable(false);
                    break;

                case GameSceneStates.GameOver:
                    WaveServices.TimerFactory.CreateTimer("GameoverTimer", TimeSpan.FromMilliseconds(200), () =>
                        {
                            this.currentScoreTB.IsVisible = false;
                            this.Pause();

                            var screenTransition = new CoverTransition(TimeSpan.FromSeconds(0.5), CoverTransition.EffectOptions.FromBotton)
                            {
                                EaseFunction = new CubicEase()
                                {
                                    EasingMode = EasingMode.EaseInOut
                                }
                            };

                            WaveServices.ScreenContextManager.Push(new ScreenContext(new GameOverScene()), screenTransition);
                        }, false);
                    break;

                default:
                    break;
            }
        }
    }
}
