using System.Runtime.Serialization;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;

namespace DeepSpace.Components.Camera
{
    [DataContract]
    public class CenterCamera2DComponent : Component
    {
        [RequiredComponent]
        protected Camera2D camera;

        public CenterCamera2DComponent() : base("CenterCamera2DComponent")
        {
        }

        protected override void DefaultValues()
        {
            base.DefaultValues();
        }

        protected override void Initialize()
        {
            base.Initialize();

            this.camera.CenterScreen();
        }
    }
}
