using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using P2PTank.Managers;
using P2PTank.Scenes;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Physics2D;

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
        public float CurrentRotationBarrelSpeed { get; set; }

        [IgnoreDataMember]
        public float CurrentShootInterval { get; private set; }

        protected override void DefaultValues()
        {
            base.DefaultValues();

            this.InitialLive = 100;
            this.InitialSpeed = 20;
            this.InitialRotationSpeed = 0.5f;
            this.InitialRotationBarrelSpeed = 0.5f;
            this.InitialShootInterval = 0.5f;
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
            this.CurrentRotationBarrelSpeed = this.InitialRotationBarrelSpeed;
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
    }
}
