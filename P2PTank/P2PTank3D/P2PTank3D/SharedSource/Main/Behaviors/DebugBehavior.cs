using P2PTank.Scenes;
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

                this.KeyPressAction(Keys.F2, () =>
                {
                    WaveServices.ScreenContextManager.SetDiagnosticsActive(true);
                });

                this.KeyPressAction(Keys.I, () =>
                {
                    var gamePlayScene = WaveServices.ScreenContextManager.CurrentContext.FindScene<GamePlayScene>();
                    gamePlayScene.TestHitMyself();
                });

                this.KeyPressAction(Keys.D, () =>
                {
                    var gamePlayScene = WaveServices.ScreenContextManager.CurrentContext.FindScene<GamePlayScene>();
                    gamePlayScene.TestDieMyself();
                    gamePlayScene.CreateCountDown();
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