using System;
using System.Runtime.Serialization;
using WaveEngine.Common.Input;
using WaveEngine.Framework;
using WaveEngine.Framework.Services;

namespace P2PTank.Behaviors
{
    [DataContract]
    public class DebugBehavior : Behavior
    {
        private Input input;
        private KeyboardState currentKeyboardState;
        private KeyboardState lastKeyboardState;

        protected override void Update(TimeSpan gameTime)
        {
            this.input = WaveServices.Input;
            this.currentKeyboardState = this.input.KeyboardState;

            if (currentKeyboardState.IsConnected)
            {
                this.KeyPressAction(Keys.F1, () =>
                {
                    this.RenderManager.DebugLines = !this.RenderManager.DebugLines;
                });
            }

            this.lastKeyboardState = currentKeyboardState;
        }

        private void KeyPressAction(Keys key, Action action)
        {
            if (currentKeyboardState.IsKeyPressed(key) &&
                   this.lastKeyboardState.IsKeyReleased(key))
            {
                action();
            }
        }
    }
}