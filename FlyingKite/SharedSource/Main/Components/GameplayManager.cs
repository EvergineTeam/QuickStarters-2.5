#region File Description
//-----------------------------------------------------------------------------
// Flying Kite
//
// Quickstarter for Wave University Tour 2014.
// Author: Wave Engine Team
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using FlyingKite.Behaviors;
using FlyingKite.Enums;
using FlyingKite.Managers;
using FlyingKite.Scenes;
using System;
using System.Runtime.Serialization;
using WaveEngine.Components.Transitions;
using WaveEngine.Components.UI;
using WaveEngine.Framework;
using WaveEngine.Framework.Animation;
using WaveEngine.Framework.Services;
#endregion

namespace FlyingKite.Components
{
    [DataContract]
    public class GameplayManager : Component, IDisposable
    {
        private SoundManager soundManager;

        private int score;

        private GameplayStates currentState;

        private KiteBehavior kiteBehavior;

        private TextBlock currentScoreTB;

        private CrashEffectComponent crashEffectComponent;

        #region Properties   
        private int CurrentScore
        {
            get
            {
                return this.score;
            }

            set
            {
                this.score = value;

                if (this.currentScoreTB != null)
                {
                    this.currentScoreTB.Text = this.score.ToString();
                }
            }
        }

        private bool IsScoreVisible
        {
            get
            {
                bool result = false;

                if (this.currentScoreTB != null)
                {
                    result = this.currentScoreTB.IsVisible;
                }

                return result;
            }

            set
            {
                if (this.currentScoreTB != null)
                {
                    this.currentScoreTB.IsVisible = value;
                }
            }
        }

        [DataMember]
        public GameplayStates InitialState
        {
            get;
            set;
        }
        #endregion

        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            this.currentScoreTB = this.EntityManager.Find<TextBlock>("currentScoreTB");

            this.kiteBehavior = this.EntityManager.Find("kite")
                                                  .FindComponent<KiteBehavior>();

            this.crashEffectComponent = this.EntityManager.Find("crashEffect")
                                                          .FindComponent<CrashEffectComponent>();
        }

        protected override void Initialize()
        {
            base.Initialize();

            if (!this.isInitialized)
            {
                this.soundManager = WaveServices.GetService<SoundManager>();

                if (WaveServices.ScreenContextManager != null)
                {
                    WaveServices.ScreenContextManager.OnActivatingScene += this.ScreenContextManager_OnActivatingScene;
                    WaveServices.ScreenContextManager.OnDesactivatingScene += this.ScreenContextManager_OnDesactivatingScene;
                }

                this.kiteBehavior.OnStateChanged += this.KiteBehavior_OnStateChanged;

                this.UpdateCustomLayerEntities();

                this.SetState(this.InitialState);
            }
        }

        public void Dispose()
        {
            WaveServices.ScreenContextManager.OnActivatingScene -= ScreenContextManager_OnActivatingScene;
            WaveServices.ScreenContextManager.OnDesactivatingScene -= ScreenContextManager_OnDesactivatingScene;

            this.kiteBehavior.OnStateChanged -= this.KiteBehavior_OnStateChanged;
        }

        private void ScreenContextManager_OnActivatingScene(Scene scene)
        {
            if (scene is MenuScene)
            {
                this.SetState(GameplayStates.Intro);
            }
        }

        private void ScreenContextManager_OnDesactivatingScene(Scene scene)
        {
            if (scene is MenuScene
            || scene is GameOverScene)
            {
                this.SetState(GameplayStates.Gameplay);
            }
        }

        private void KiteBehavior_OnStateChanged(object sender, KiteStates newState)
        {
            switch (newState)
            {
                case KiteStates.CaptureStar:
                    if (this.soundManager != null)
                    {
                        this.soundManager.PlaySound(SoundManager.SOUNDS.Coin);
                    }

                    this.CurrentScore++;
                    break;

                case KiteStates.Crash:
                    if (this.soundManager != null)
                    {
                        this.soundManager.PlaySound(SoundManager.SOUNDS.Crash);
                    }

                    // Crash Effect
                    this.crashEffectComponent
                        .DoEffect();

                    //Dissable Scroll
                    this.SetScrollEnable(false);
                    break;

                case KiteStates.GameOver:
                    this.SetState(GameplayStates.GameOver);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Updates entities with custom layers.
        /// </summary>
        private void UpdateCustomLayerEntities()
        {
            //this.RenderManager.RegisterLayerAfter(new KiteLayer(this.RenderManager), DefaultLayers.Alpha);

            //this.Assets.LoadModel<MaterialModel>(WaveContent.Assets.Gameplay.Materials.KiteRopeMaterial).Material.LayerType = CustomLayers.Kite;

            //// Game Entities
            //this.EntityManager.Find("kiteBall")
            //                  .FindComponent<SpriteAtlasRenderer>()
            //                  .LayerType = CustomLayers.Kite;
        }

        /// <summary>
        /// Resets the scene. Used to restart the game.
        /// </summary>
        private void ResetScene()
        {
            foreach (Entity obstaclePair in this.EntityManager.FindAllByTag("OBSTACLE"))
            {
                var obstaclePairComponent = obstaclePair.FindComponent<ObstaclePairComponent>();

                obstaclePairComponent.ResetState();
            }
        }

        /// <summary>
        /// Enable or disable the ScrollBehavior of all the entities inside the EntityManger of the scene.
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        private void SetScrollEnable(bool value)
        {
            foreach (var entity in this.EntityManager.AllEntities)
            {
                var scrollBehavior = entity.FindComponent<ScrollBehavior>();

                if (scrollBehavior != null)
                {
                    scrollBehavior.IsActive = value;
                }
            }
        }

        /// <summary>
        /// Do game over transition.
        /// </summary>
        private void GameOverTransition()
        {
            var notRunningInEditorMode = this.Owner.Scene is GameScene;

            if (notRunningInEditorMode)
            {

                this.IsScoreVisible = false;
                this.Owner.Scene.Pause();

                var screenTransition = new CoverTransition(TimeSpan.FromSeconds(0.5), CoverTransition.EffectOptions.FromBotton)
                {
                    EaseFunction = new CubicEase()
                    {
                        EasingMode = EasingMode.EaseInOut
                    }
                };

                WaveServices.ScreenContextManager.Push(new ScreenContext(new GameOverScene(this.CurrentScore)), screenTransition);
            }
            else
            {
                this.SetState(GameplayStates.Gameplay);
            }
        }

        /// <summary>
        /// Changes the scene state.
        /// </summary>
        /// <param name="state">The state.</param>
        public void SetState(GameplayStates state)
        {
            var previousState = this.currentState;
            this.currentState = state;

            switch (state)
            {
                case GameplayStates.Intro:
                    this.IsScoreVisible = false;

                    foreach (Entity obstaclePair in this.EntityManager.FindAllByTag("OBSTACLE"))
                    {
                        obstaclePair.Enabled = false;
                    }

                    this.kiteBehavior.SetState(KiteStates.TakeOff);
                    break;

                case GameplayStates.Gameplay:
                    this.IsScoreVisible = true;
                    this.CurrentScore = 0;
                    this.kiteBehavior.SetState(KiteStates.Gameplay);

                    this.ResetScene();
                    this.SetScrollEnable(true);

                    this.Owner.Scene.Resume();
                    break;

                case GameplayStates.GameOver:
                    WaveServices.TimerFactory.CreateTimer("GameoverTimer", TimeSpan.FromMilliseconds(200), this.GameOverTransition, false);
                    break;

                default:
                    break;
            }
        }
    }
}
