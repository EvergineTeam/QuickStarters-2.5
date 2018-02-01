using System.Runtime.Serialization;
using WaveEngine.Common.Graphics;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Components.Graphics3D;
using WaveEngine.Framework;
using WaveEngine.Materials;

namespace P2PTank.Components
{
    [DataContract]
    public class BulletComponent : Component
    {
        private Color color;

        [RequiredComponent]
        private Sprite sprite = null;

        [DataMember]
        public float InitialSpeed { get; set; }

        [IgnoreDataMember]
        public float CurrentSpeed { get; private set; }

        [DataMember]
        public Color Color
        {
            get
            {
                return this.color;
            }
            set
            {
                this.color = value;
                this.UpdateColor();
            }
        }

        protected override void DefaultValues()
        {
            base.DefaultValues();

            this.InitialSpeed = 80;     // Bullet speed
            this.Color = Color.White;
        }

        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            this.CurrentSpeed = this.InitialSpeed;

            this.UpdateColor();
        }

        protected override void Initialize()
        {
            base.Initialize();

            var model3D = Owner.FindChild("model3D", true);

            if (model3D != null)
            {
                var material = model3D.FindComponent<MaterialComponent>();
                material.OnComponentInitialized += MaterialOnComponentInitialized;
            }
        }

        private void MaterialOnComponentInitialized(object sender, System.EventArgs e)
        {
            var material = (MaterialComponent)sender;
            ((StandardMaterial)material.Material).DiffuseColor = this.color;
            material.OnComponentInitialized -= MaterialOnComponentInitialized;
        }

        private void UpdateColor()
        {
            if (this.sprite != null)
            {
                this.sprite.TintColor = this.Color;
            }
        }
    }
}
