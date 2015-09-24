using SurvivorNinja.Behaviors;
using SurvivorNinja.Commons;
using SurvivorNinja.Entities;
using SurvivorNinja.Managers;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using WaveEngine.Common.Input;
using WaveEngine.Common.Media;
using WaveEngine.Components;
using WaveEngine.Components.UI;
using WaveEngine.Framework;
using WaveEngine.Framework.Services;

namespace SurvivorNinja.Components
{
    public enum States
    {
        Menu,
        GamePlay,
        GameOver,
    };

    [DataContract(Namespace = "SurvivorNinja.Components")]
    public class GamePlayManager : Behavior
    {
        private PlayerBehavior player;
        private States currentState;

        private HubPanel hubPanel;
        private Entity logo, tap_to_start, game_over;
        private TextBlock bestScores;        
        private EnemyEmitter enemyEmitter;
        private Joystick leftJoystick, rightJoystick;

        #region Properties
        /// <summary>
        /// Gets or sets the state of the current.
        /// </summary>
        public States CurrentState
        {
            get { return currentState; }
            set
            {
                if (!this.Owner.IsDisposed)
                {
                    this.UpdateState(value);
                }
            }
        }
        #endregion
        
        protected override void Initialize()
        {
            base.Initialize();

            this.logo = this.EntityManager.Find("Logo");
            this.tap_to_start = this.EntityManager.Find("TapToStart");
            this.game_over = this.EntityManager.Find("GameOver");
            this.bestScores = this.EntityManager.Find<TextBlock>("BestScores");

            this.player = this.EntityManager.Find("Player").FindComponent<PlayerBehavior>();
            this.enemyEmitter = this.EntityManager.Find("EnemyEmitter").FindComponent<EnemyEmitter>();
            this.leftJoystick = this.EntityManager.Find<Joystick>("leftJoystick");
            this.rightJoystick = this.EntityManager.Find<Joystick>("rightJoystick");
            this.hubPanel = this.EntityManager.Find<HubPanel>("HubPanel");

            this.CurrentState = States.Menu;
        }

        protected override void Update(TimeSpan gameTime)
        {
            if (this.CurrentState == States.Menu)
            {
                Input input = WaveServices.Input;

                if ((input.TouchPanelState.IsConnected && input.TouchPanelState.Count > 0) ||
                    (input.KeyboardState.IsConnected &&
                    (input.KeyboardState.Space == ButtonState.Pressed ||
                     input.KeyboardState.A == ButtonState.Pressed ||
                     input.KeyboardState.S == ButtonState.Pressed ||
                     input.KeyboardState.D == ButtonState.Pressed ||
                     input.KeyboardState.W == ButtonState.Pressed ||
                     input.KeyboardState.Up == ButtonState.Pressed ||
                     input.KeyboardState.Down == ButtonState.Pressed ||
                     input.KeyboardState.Left == ButtonState.Pressed ||
                     input.KeyboardState.Right == ButtonState.Pressed)))
                {
                    this.CurrentState = States.GamePlay;
                }
            }
            else if (this.CurrentState == States.GamePlay)
            {
                if (this.player.Life <= 0)
                {
                    this.CurrentState = States.GameOver;
                }
            }

        }

        /// <summary>
        /// Updates the state.
        /// </summary>
        /// <param name="newState">The new state.</param>
        private void UpdateState(States newState)
        {
            this.logo.IsVisible = false;
            this.tap_to_start.IsVisible = false;
            this.game_over.IsVisible = false;

            this.enemyEmitter.IsActive = false;

            if (this.bestScores != null)    this.bestScores.IsVisible = false;
            if (this.hubPanel != null)      this.hubPanel.IsVisible = false;
            if (this.leftJoystick != null)  this.leftJoystick.IsActive = false;
            if (this.rightJoystick != null) this.rightJoystick.IsActive = false;

            switch (newState)
            {
                case States.Menu:
                    this.ResetMatch();
                    this.logo.IsVisible = true;
                    this.tap_to_start.IsVisible = true;
                    if (this.hubPanel != null) this.hubPanel.IsVisible = false;
                    if (this.bestScores != null) this.bestScores.IsVisible = true;
                    break;
                case States.GamePlay:
                    SoundsManager.Instance.PlaySound(SoundsManager.SOUNDS.Ready);
                    if (this.hubPanel != null) this.hubPanel.IsVisible = true;
                    if (this.bestScores != null) this.bestScores.IsVisible = false;
                    this.enemyEmitter.IsActive = true;
                    WaveServices.TimerFactory.CreateTimer("gamePlayTimer", TimeSpan.FromSeconds(0.5f), () =>
                    {
                        if (this.leftJoystick != null) this.leftJoystick.IsActive = true;
                        if (this.rightJoystick != null) this.rightJoystick.IsActive = true;
                        this.player.IsActive = true;
                    }, false);
                    break;
                case States.GameOver:
                    SoundsManager.Instance.PlaySound(SoundsManager.SOUNDS.GameOver);
                    this.game_over.IsVisible = true;
                    this.player.IsActive = false;

                    WaveServices.TimerFactory.CreateTimer("gameOverTimer", TimeSpan.FromSeconds(2f), () =>
                    { 
                        this.CurrentState = States.Menu;
                    }, false);
                    break;
                default:
                    break;
            }

            this.currentState = newState;
        }

        /// <summary>
        /// Resets the match.
        /// </summary>
        private void ResetMatch()
        {
            if (this.hubPanel == null)
            {
                return;
            }

            GameStorage gameStorage = Catalog.GetItem<GameStorage>();

            if (this.hubPanel.Murders > gameStorage.BestScore)
            {
                gameStorage.BestScore = this.hubPanel.Murders;
                WaveServices.Storage.Write<GameStorage>(gameStorage);
            }

            this.bestScores.Text = string.Format("BestScores: {0}", gameStorage.BestScore);

            this.player.Reset();
            this.enemyEmitter.Reset();
            this.hubPanel.Reset();
        }
    }
}
