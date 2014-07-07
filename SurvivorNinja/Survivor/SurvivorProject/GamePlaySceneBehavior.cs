#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Input;
using WaveEngine.Framework;
using WaveEngine.Framework.Services; 
#endregion

namespace SurvivorProject
{
    public class GamePlaySceneBehavior : SceneBehavior
    {
        private GamePlayScene gamePlayScene;

        /// <summary>
        /// Resolves the dependencies needed for this instance to work.
        /// </summary>
        protected override void ResolveDependencies()
        {
            this.gamePlayScene = this.Scene as GamePlayScene;
        }

        /// <summary>
        /// Allows this instance to execute custom logic during its <c>Update</c>.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        /// <remarks>
        /// This method will not be executed if it are not <c>Active</c>.
        /// </remarks>
        protected override void Update(TimeSpan gameTime)
        {
            if (this.gamePlayScene.CurrentState == GamePlayScene.States.Menu)
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
                    this.gamePlayScene.CurrentState = GamePlayScene.States.GamePlay;
                }
            }
        }
    }
}
