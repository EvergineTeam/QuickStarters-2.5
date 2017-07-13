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

        public PlayerInputBehavior(P2PManager p2pManager)
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

            if (input.KeyboardState.Up == ButtonState.Pressed)
            {
                this.Move(true, elapsedTime);
            }
            else if (input.KeyboardState.Down == ButtonState.Pressed)
            {
                this.Move(false, elapsedTime);
            }

            if (input.KeyboardState.Left == ButtonState.Pressed)
            {
                this.Rotate(true, elapsedTime);
            }
            else if (input.KeyboardState.Right == ButtonState.Pressed)
            {
                this.Rotate(false, elapsedTime);
            }
        }

        private async void Move(bool forward, float elapsedTime)
        {
            var orientation = this.transform.Orientation;
            this.transform.LocalPosition += (forward ? -1 : 1) * (orientation * Vector3.UnitY * elapsedTime * this.tankComponent.CurrentSpeed).ToVector2();

            var moveMessage = new MoveMessage()
            {
                PlayerId = "Player1",
                X = this.transform.LocalPosition.X,
                Y = this.transform.LocalPosition.Y,
            };

            await this.p2pManager.SendBroadcastAsync(this.p2pManager.CreateMessage(P2PMessageType.Move, moveMessage));
        }

        private void Rotate(bool left, float elapsedTime)
        {
            var roll = (left ? -1 : 1) * this.tankComponent.CurrentRotationSpeed * elapsedTime;
            this.transform.Orientation = this.transform.Orientation * Quaternion.CreateFromYawPitchRoll(0.0f, 0.0f, roll);
        }
    }
}
