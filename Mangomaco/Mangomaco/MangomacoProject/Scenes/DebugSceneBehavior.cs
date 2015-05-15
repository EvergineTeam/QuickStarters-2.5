#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using WaveEngine.Common.Input;
using WaveEngine.Framework;
using WaveEngine.Framework.Services;

#endregion
namespace MangomacoProject.Scenes
{
    /// <summary>
    /// Debug Scene Behavior
    /// </summary>
    public class DebugSceneBehavior : SceneBehavior
    {
        private Input inputService;
        private KeyboardState beforeKeyboardState;
        private static bool globalDiagnostics;
        private static bool globalDebugLines;
        private static bool globalDebugElemVisibility = true;

        private bool localDiagnostics;
        private bool localDebugLines;
        private bool localDebugElemVisibility;

        /// <summary>
        /// Initializes a new instance of the <see cref="DebugSceneBehavior" /> class.
        /// </summary>
        public DebugSceneBehavior()
            : base("BebugSceneBehavior")
        {
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

            var firstScene = this.GetScenes().First(scene => !scene.IsPaused);
            var isFirstScene = this.Scene == firstScene;

            if (isFirstScene && inputService.KeyboardState.IsConnected)
            {
                // Key F1
                if (inputService.KeyboardState.F1 == ButtonState.Pressed &&
                    beforeKeyboardState.F1 != ButtonState.Pressed)
                {
                    globalDiagnostics = !globalDiagnostics;
                }

                // Key F3
                if (inputService.KeyboardState.F3 == ButtonState.Pressed &&
                    beforeKeyboardState.F3 != ButtonState.Pressed)
                {
                    globalDebugLines = !globalDebugLines;
                }

                // Key F4
                if (inputService.KeyboardState.F4 == ButtonState.Pressed &&
                    beforeKeyboardState.F4 != ButtonState.Pressed)
                {
                    globalDebugElemVisibility = !globalDebugElemVisibility;
                }
            }

            this.UpdateDebugFlags(isFirstScene);

            beforeKeyboardState = inputService.KeyboardState;
        }

        /// <summary>
        /// Gets the scenes.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<Scene> GetScenes()
        {
            var screenContext = WaveServices.ScreenContextManager.CurrentContext;

            for (int i = 0; i < screenContext.Count; i++)
            {
                yield return screenContext[i];
            }
        }

        /// <summary>
        /// Updates the debug flags.
        /// </summary>
        /// <param name="isFirstScene">if set to <c>true</c> [is first scene].</param>
        private void UpdateDebugFlags(bool isFirstScene)
        {
            if (isFirstScene)
            {
                if (this.localDiagnostics != globalDiagnostics)
                {
                    this.localDiagnostics = globalDiagnostics;
                    WaveServices.ScreenContextManager.SetDiagnosticsActive(this.localDiagnostics);
                }

                if (this.localDebugLines != globalDebugLines)
                {
                    this.localDebugLines = globalDebugLines;

                    this.Scene.RenderManager.DebugLines = this.localDebugLines;
                }
            }

            if (this.localDebugElemVisibility != globalDebugElemVisibility)
            {
                this.localDebugElemVisibility = globalDebugElemVisibility;
                foreach (var debugItem in this.Scene.EntityManager.FindAllByTag("DEBUG_ELEM"))
                {
                    Entity debugItemEntity = debugItem as Entity;

                    if (debugItemEntity == null)
                    {
                        debugItemEntity = (debugItem as BaseDecorator).Entity;
                    }

                    debugItemEntity.IsVisible = this.localDebugElemVisibility;
                }
            }
        }
    }
}
