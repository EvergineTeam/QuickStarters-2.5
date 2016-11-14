using System;
using System.Runtime.Serialization;
using WaveEngine.Common.Input;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Diagnostic;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Physics2D;
using WaveEngine.Framework.Services;

namespace SuperSlingshot.Behaviors
{
    [DataContract]
    public class DebugMovementBehavior : Behavior
    {
        private Input input;
        private Vector2 movement;
        private KeyboardState lastKeyboardState;
        private bool diagnostics;

        [RequiredComponent]
        private Transform2D transform = null;

        [RequiredComponent]
        private RigidBody2D rigidBody = null;

        [DataMember]
        public float Speed { get; set; }

        protected override void Update(TimeSpan gameTime)
        {
            this.input = WaveServices.Input;

            this.movement.X = 0;
            this.movement.Y = 0;

            KeyboardState currentKeyboardState = this.input.KeyboardState;
            if (currentKeyboardState.IsConnected)
            {
                if (currentKeyboardState.IsKeyPressed(Keys.O) &&
                   this.lastKeyboardState.IsKeyReleased(Keys.O))
                {
                    this.RenderManager.DebugLines = !this.RenderManager.DebugLines;
                }

                if (currentKeyboardState.IsKeyPressed(Keys.P) &&
                   this.lastKeyboardState.IsKeyReleased(Keys.P))
                {
                    this.diagnostics = !this.diagnostics;
                    WaveServices.ScreenContextManager.SetDiagnosticsActive(this.diagnostics);
                }

                if (currentKeyboardState.IsKeyPressed(Keys.Up))
                {
                    this.movement.Y = -1;
                }

                if (currentKeyboardState.IsKeyPressed(Keys.Down))
                {
                    this.movement.Y = 1;
                }
                if (currentKeyboardState.IsKeyPressed(Keys.Right))
                {
                    this.movement.X = 1;
                }
                if (currentKeyboardState.IsKeyPressed(Keys.Left))
                {
                    this.movement.X = -1;
                }

                if (currentKeyboardState.IsKeyPressed(Keys.Space) &&
                   this.lastKeyboardState.IsKeyReleased(Keys.Space))
                {
                    this.rigidBody.ApplyLinearImpulse(new Vector2(3, -4), Vector2.Zero, true);
                }

                this.lastKeyboardState = currentKeyboardState;
            }

            this.rigidBody.ApplyLinearImpulse(this.movement * 0.01f, Vector2.Zero, true);
            //this.rigidBody.Transform2D.Position += this.movement * this.Speed * (float)gameTime.TotalSeconds;
            //this.transform.Position += this.movement * this.Speed * (float)gameTime.TotalSeconds;

            Labels.Add(this.Owner.Name + " transform position", this.transform.Position);
        }
    }
}
