#region Using Statements
using System;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Input;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services; 
#endregion

namespace BasketKing.Commons
{
    public class DebugSceneBehavior : SceneBehavior
    {
        private Input inputService;
        private KeyboardState beforeKeyboardState;
        private bool diagnostics;
        private bool wireframe;

        /// <summary>
        /// Initializes a new instance of the <see cref="DebugSceneBehavior" /> class.
        /// </summary>
        public DebugSceneBehavior()
            : base("DebugSceneBehavior")
        {
            this.diagnostics = false;
            WaveServices.ScreenContextManager.SetDiagnosticsActive(this.diagnostics);

            this.wireframe = false;
            WaveServices.GraphicsDevice.RenderState.FillMode = FillMode.Solid;
        }

        /// <summary>
        /// Resolves the dependencies needed for this instance to work.
        /// </summary>
        protected override void ResolveDependencies()
        {
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
             inputService = WaveServices.Input;

             if (inputService.KeyboardState.IsConnected)
             {
                 // Key F1
                 if (inputService.KeyboardState.F1 == ButtonState.Pressed &&
                     beforeKeyboardState.F1 != ButtonState.Pressed)
                 {
                     this.diagnostics = !this.diagnostics;
                     WaveServices.ScreenContextManager.SetDiagnosticsActive(this.diagnostics);
                     this.Scene.RenderManager.DebugLines = this.diagnostics;
                 }                 
             }

             beforeKeyboardState = inputService.KeyboardState;
        }
    }
}