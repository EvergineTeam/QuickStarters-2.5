using P2PTank.Entities;
using WaveEngine.Common.Math;
using WaveEngine.Components.Cameras;
using WaveEngine.Framework;

namespace P2PTank.Scenes
{
    public class VirtualJoystickScene : Scene
    {
        public Joystick Joystick { get; private set; }
        public FireButton FireButton { get; private set; }


        protected override void CreateScene()
        {
            FixedCamera2D camera = new FixedCamera2D("camera")
            {
                ClearFlags = WaveEngine.Common.Graphics.ClearFlags.DepthAndStencil
            };

            this.EntityManager.Add(camera);

            var vm = this.VirtualScreenManager;
            float width = vm.RightEdge - vm.LeftEdge;
            float halfWidth = width / 2;
            float height = vm.BottomEdge - vm.TopEdge;

            this.Joystick = new Joystick("joystick", new RectangleF(vm.LeftEdge, vm.TopEdge, halfWidth, height));
            this.EntityManager.Add(this.Joystick);

            this.FireButton = new FireButton("fireButton", new RectangleF(vm.LeftEdge + (halfWidth), vm.TopEdge, halfWidth, height));
            this.EntityManager.Add(this.FireButton);
        }
    }
}