using System;
using System.Collections.Generic;
using System.Text;
using P2PTank.Components;
using P2PTank.Entities.P2PMessages;
using P2PTank.Managers;
using WaveEngine.Common.Input;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;

namespace P2PTank.Behaviors
{
    public class PlayerInputBehavior : Behavior
    {
        [RequiredComponent]
        private TankComponent tankComponent = null;

        [RequiredComponent]
        private Transform2D transform = null;

        private P2PManager p2pManager;

        public PlayerInputBehavior(P2PManager p2pManager = null)
        {
            this.p2pManager = p2pManager;
        }

        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();
        }

        protected override void Update(TimeSpan gameTime)
        {
            Input input = WaveServices.Input;

            float elapsedTime = (float)gameTime.TotalSeconds;

            this.HandleKeyboard(input, elapsedTime);
            this.HandlePad(input, elapsedTime);

        }

        private void HandlePad(Input input, float elapsedTime)
        {
            GamePadState gamepadState = input.GamePadState;

            if (gamepadState.IsConnected)
            {
                Vector2 leftThumb = this.ApplyDeadZone(gamepadState.ThumbStricks.Left);
                Vector2 rightthumb = this.ApplyDeadZone(gamepadState.ThumbStricks.Right);

                if (leftThumb != Vector2.Zero)
                {
                    this.Move(-leftThumb.Y, elapsedTime);
                    this.Rotate(leftThumb.X, elapsedTime);
                }

                if (rightthumb != Vector2.Zero)
                {
                    this.RotateBarrel(rightthumb.X, elapsedTime);
                }
            }
        }

        private Vector2 ApplyDeadZone(Vector2 vector)
        {
            var output = vector;

            var deadZone = GameSettings.GamePadDeadZone;

            // Scaled radial DeadZone, the Right way to do a DeadZone:
            // https://web.archive.org/web/20130418234531/http://www.gamasutra.com/blogs/JoshSutphin/20130416/190541/Doing_Thumbstick_Dead_Zones_Right.php
            var magnitude = output.Length();
            if (magnitude < deadZone)
            {
                output = Vector2.Zero;
            }
            else
            {
                output.Normalize();
                output *= ((magnitude - deadZone) / (1 - deadZone));
            }

            return output;
        }

        private void HandleKeyboard(Input input, float elapsedTime)
        {
            if (input.KeyboardState.Up == ButtonState.Pressed)
            {
                this.Move(-1.0f, elapsedTime);
            }
            else if (input.KeyboardState.Down == ButtonState.Pressed)
            {
                this.Move(1.0f, elapsedTime);
            }

            if (input.KeyboardState.Left == ButtonState.Pressed)
            {
                this.Rotate(-1.0f, elapsedTime);
            }
            else if (input.KeyboardState.Right == ButtonState.Pressed)
            {
                this.Rotate(1.0f, elapsedTime);
            }

            if (input.KeyboardState.A == ButtonState.Pressed)
            {
                this.RotateBarrel(-1.0f, elapsedTime);
            }
            else if (input.KeyboardState.D == ButtonState.Pressed)
            {
                this.RotateBarrel(1.0f, elapsedTime);
            }
        }

        private async void Move(float forward, float elapsedTime)
        {
            this.tankComponent.Move(forward, elapsedTime);

            if (this.p2pManager != null)
            {
                var moveMessage = new MoveMessage()
                {
                    PlayerId = "Player1",
                    X = this.transform.LocalPosition.X,
                    Y = this.transform.LocalPosition.Y,
                };

                await this.p2pManager.SendBroadcastAsync(this.p2pManager.CreateMessage(P2PMessageType.Move, moveMessage));
            }
        }

        private void Rotate(float left, float elapsedTime)
        {
            this.tankComponent.Rotate(left, elapsedTime);
        }
        private void RotateBarrel(float left, float elapsedTime)
        {
            this.tankComponent.RotateBarrel(left, elapsedTime);
        }

    }
}
