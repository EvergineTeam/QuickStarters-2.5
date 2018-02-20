using System.Runtime.Serialization;
using WaveEngine.Common.Graphics;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Framework;

namespace P2PTank.Components
{
    [DataContract]
    public class TankComponent : Component
    {
        private Color color;

        private Sprite barrel;

        private Sprite body;

        [DataMember]
        public float InitialLive { get; set; }

        [DataMember]
        public float InitialSpeed { get; set; }

        [DataMember]
        public float InitialRotationSpeed { get; set; }

        [DataMember]
        public float InitialRotationBarrelSpeed { get; set; }

        [DataMember]
        public float InitialShootInterval { get; set; }

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

        [IgnoreDataMember]
        public float CurrentLive { get; set; }

        [IgnoreDataMember]
        public float CurrentSpeed { get; set; }

        [IgnoreDataMember]
        public float CurrentRotationSpeed { get; set; }

        [IgnoreDataMember]
        public float CurrentShootInterval { get; set; }

        protected override void DefaultValues()
        {
            base.DefaultValues();

            this.InitialLive = 100; // By default, every impact reduce 50. 
            this.InitialSpeed = 40;
            this.InitialRotationSpeed = 0.5f;
            this.InitialRotationBarrelSpeed = 0.5f;
            this.InitialShootInterval = 0.50f;
            this.Color = Color.White;
        }

        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            this.body = this.Owner.FindChild(GameConstants.EntityNameTankBody).FindComponent<Sprite>();
            this.barrel = this.Owner.FindChild(GameConstants.EntitynameTankBarrel).FindComponent<Sprite>();

            this.CurrentLive = this.InitialLive;
            this.CurrentSpeed = this.InitialSpeed;
            this.CurrentRotationSpeed = this.InitialRotationSpeed;
            this.CurrentShootInterval = this.InitialShootInterval;

            this.UpdateColor();
        }

        private void UpdateColor()
        {
            if (this.barrel != null)
            {
                this.barrel.TintColor = this.color;
            }

            if (this.body != null)
            {
                this.body.TintColor = this.color;
            }
        }

        internal void ResetPowerUp()
        {
            this.CurrentShootInterval = this.InitialShootInterval;
        }
    }
}
