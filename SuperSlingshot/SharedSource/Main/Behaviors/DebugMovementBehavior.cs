using System;
using System.Runtime.Serialization;
using SuperSlingshot.Components;
using SuperSlingshot.Scenes;
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

                if (currentKeyboardState.IsKeyPressed(Keys.R) &&
                   this.lastKeyboardState.IsKeyReleased(Keys.R))
                {
                    var entity = this.Owner.FindComponent<PlayerComponent>();
                    if (entity != null)
                    {
                        entity.PrepareToLaunch();
                    }
                }

                if (currentKeyboardState.IsKeyPressed(Keys.Number1) &&
                   this.lastKeyboardState.IsKeyReleased(Keys.Number1))
                {
                    var entity = this.Owner.FindComponent<PlayerBehavior>();
                    if (entity != null)
                    {
                        entity.PlayerState = Enums.PlayerState.Prepared;
                    }
                }

                if (currentKeyboardState.IsKeyPressed(Keys.Number2) &&
                   this.lastKeyboardState.IsKeyReleased(Keys.Number2))
                {
                    var entity = this.Owner.FindComponent<PlayerBehavior>();
                    if (entity != null)
                    {
                        entity.PlayerState = Enums.PlayerState.InTheAir;
                    }
                }

                if (currentKeyboardState.IsKeyPressed(Keys.Number3) &&
                   this.lastKeyboardState.IsKeyReleased(Keys.Number3))
                {
                    var entity = this.Owner.FindComponent<PlayerBehavior>();
                    if (entity != null)
                    {
                        entity.PlayerState = Enums.PlayerState.Stamped;
                    }
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

                if (currentKeyboardState.IsKeyPressed(Keys.Q) &&
                    this.lastKeyboardState.IsKeyReleased(Keys.Q))
                {
                    WaveServices.ScreenContextManager.Push(new ScreenContext(new MenuScene()));
                }

                this.lastKeyboardState = currentKeyboardState;
            }

            //this.rigidBody.ApplyLinearImpulse(this.movement * 0.01f, Vector2.Zero, true);
            this.rigidBody.ApplyTorque(this.movement.X * this.Speed);
            //this.rigidBody.Transform2D.Position += this.movement * this.Speed * (float)gameTime.TotalSeconds;
            //this.transform.Position += this.movement * this.Speed * (float)gameTime.TotalSeconds;

            Labels.Add(this.Owner.Name + " transform position", this.transform.Position);
        }
    }
}
