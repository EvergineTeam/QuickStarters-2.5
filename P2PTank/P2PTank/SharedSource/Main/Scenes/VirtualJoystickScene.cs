using P2PTank.Entities;
using WaveEngine.Common.Math;
using WaveEngine.Framework;

namespace P2PTank.Scenes
{
    public class VirtualJoystickScene : Scene
    {
        protected override void CreateScene()
        {
            this.CreateVirtualJoysticks();
        }

        private void CreateVirtualJoysticks()
        {
            // Left Joystick
            RectangleF leftArea = new RectangleF(
                0,
                0,
                this.VirtualScreenManager.VirtualWidth / 2f,
                this.VirtualScreenManager.VirtualHeight);
            var leftJoystick = new Joystick("leftJoystick", leftArea);

            EntityManager.Add(leftJoystick);

            // Right Joystick
            RectangleF rightArea = new RectangleF(
                this.VirtualScreenManager.VirtualWidth / 2,
                0,
                this.VirtualScreenManager.VirtualWidth / 2f,
                this.VirtualScreenManager.VirtualHeight);
            var rightJoystick = new Joystick("rightJoystick", rightArea);

            EntityManager.Add(rightJoystick);
        }
    }
}
