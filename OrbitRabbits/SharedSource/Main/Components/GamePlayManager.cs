using OrbitRabbits.Entities;
using OrbitRabbits.Entities.Behaviors;
using OrbitRabbits.Managers;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using WaveEngine.Components.Gestures;
using WaveEngine.Components.UI;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;

namespace OrbitRabbits.Components
{
    [DataContract(Namespace = "OrbitRabbits.Components")]
    public class GamePlayManager : Component
    {
        private enum States
        {
            Paused,
            Play,
        };

        private States currentState;

        public Entity DetaultCamera2D
        {
            get
            {
                return this.EntityManager.Find("defaultCamera2D");
            }
        }

        public Entity TapHand
        {
            get
            {
                return this.EntityManager.Find("TapHand");
            }
        }


        public Button RestartButton
        {
            get
            {
                return this.EntityManager.Find<Button>("RestartButton");
            }
        }

        public ScoreComponent ScorePanel
        {
            get
            {
                return this.EntityManager.FindComponentFromEntityPath<ScoreComponent>("scorePanel");
            }
        }

        public RabbitEmitterBehavior RabbitEmitter
        {
            get
            {
                return this.EntityManager.Find("RabbitEmitter").FindComponent<RabbitEmitterBehavior>();
            }
        }

        public TouchGestures TouchPanel
        {
            get
            {
                return this.EntityManager.Find("Background").FindComponent<TouchGestures>();
            }
        }

        protected override void DefaultValues()
        {
            base.DefaultValues();
            this.currentState = States.Paused;
        }

        protected override void Initialize()
        {
            base.Initialize();

            this.RenderManager.ActiveCamera2D.CenterScreen();

            this.TouchPanel.TouchPressed += this.OnTouchPressed;

            if (this.RestartButton != null)
            {
                this.RestartButton.Click += this.RestartButton_Click;
            }

            this.RabbitEmitter.ScoreChanged += this.OnScoreChanged;
        }

        /// <summary>
        /// Sets the new state.
        /// </summary>
        /// <param name="state">The state.</param>
        private void SetNewState(States state)
        {
            switch (state)
            {
                case States.Paused:

                    this.TapHand.IsVisible = true;
                    this.RabbitEmitter.IsActive = false;
                    this.RabbitEmitter.Clear();
                    break;
                case States.Play:

                    this.TapHand.IsVisible = false;
                    this.RabbitEmitter.IsActive = true;
                    this.RabbitEmitter.AddRabbit();

                    SoundsManager.Instance.PlaySound(SoundsManager.SOUNDS.Start);
                    break;
            }

            this.currentState = state;
        }

        private void OnScoreChanged(object sender, int e)
        {
            if (this.ScorePanel != null)
            {
                this.ScorePanel.Scores = e;
            }
        }

        private void OnTouchPressed(object sender, GestureEventArgs e)
        {
            switch (this.currentState)
            {
                case States.Paused:
                    this.SetNewState(States.Play);
                    break;
                case States.Play:

                    this.RabbitEmitter.ApplyImpulseToLast();
                    break;
            }
        }

        private void RestartButton_Click(object sender, EventArgs e)
        {
            this.SetNewState(States.Paused);
            SoundsManager.Instance.PlaySound(SoundsManager.SOUNDS.Click);
        }
    }
}
