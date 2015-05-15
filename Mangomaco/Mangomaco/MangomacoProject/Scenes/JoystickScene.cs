#region Using Statements
using MangomacoProject.Entities;
using WaveEngine.Common.Math;
using WaveEngine.Components.Cameras;
using WaveEngine.Framework;
using WaveEngine.Framework.Services; 
#endregion

namespace MangomacoProject.Scenes
{
    /// <summary>
    /// Joystick Scene class
    /// </summary>
    public class JoystickScene : Scene
    {
        #region Properties

        /// <summary>
        /// Gets the joystick.
        /// </summary>
        public Joystick Joystick { get; private set; }

        /// <summary>
        /// Gets the jump button.
        /// </summary>
        public JumpButton JumpButton { get; private set; } 

        #endregion

        /// <summary>
        /// Creates the scene.
        /// </summary>
        /// <remarks>
        /// This method is called before all <see cref="T:WaveEngine.Framework.Entity" /> instances in this instance are initialized.
        /// </remarks>
        protected override void CreateScene()
        {
            FixedCamera2D camera = new FixedCamera2D("camera")
            {
                ClearFlags = WaveEngine.Common.Graphics.ClearFlags.DepthAndStencil
            };

            this.EntityManager.Add(camera);

            ViewportManager vm = WaveServices.ViewportManager;
            float width = vm.RightEdge - vm.LeftEdge;
            float halfWidth = width / 2;
            float height = vm.BottomEdge - vm.TopEdge;

            this.Joystick = new Joystick("joystick", new RectangleF(vm.LeftEdge, vm.TopEdge, halfWidth, height));
            this.EntityManager.Add(this.Joystick);

            this.JumpButton = new JumpButton("jumpButton", new RectangleF(vm.LeftEdge + (halfWidth), vm.TopEdge, halfWidth, height));
            this.EntityManager.Add(this.JumpButton);
        }        
    }
}
