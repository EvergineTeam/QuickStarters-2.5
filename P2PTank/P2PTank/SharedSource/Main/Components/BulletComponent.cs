using System.Runtime.Serialization;
using WaveEngine.Common.Graphics;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Framework;

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

        private void UpdateColor()
        {
            if (this.sprite != null)
            {
                this.sprite.TintColor = this.Color;
            }
        }
    }
}
