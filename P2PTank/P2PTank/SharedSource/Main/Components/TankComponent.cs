using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;

namespace P2PTank.Components
{
    [DataContract]
    public class TankComponent : Component
    {
        private Color color;

        private Sprite barrel;

        private Sprite body;

        private Transform2D barrelTransform = null;

        [RequiredComponent]
        private Transform2D transform = null;

        [DataMember]
        public float InitialLive { get; set; }

        [DataMember]
        public float InitialSpeed { get; set; }

        [DataMember]
        public float InitialRotationSpeed { get; set; }

        [DataMember]
        public float InitialRotationBarrelSpeed { get; set; }

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
        public float CurrentLive { get; private set; }

        [IgnoreDataMember]
        public float CurrentSpeed { get; private set; }

        [IgnoreDataMember]
        public float CurrentRotationSpeed { get; private set; }

        [IgnoreDataMember]
        public float CurrentRotationBarrelSpeed { get; private set; }

        protected override void DefaultValues()
        {
            base.DefaultValues();

            this.InitialLive = 100;
            this.InitialSpeed = 200;
            this.InitialRotationSpeed = 0.5f;
            this.InitialRotationBarrelSpeed = 0.5f;
            this.Color = Color.White;
        }

        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            var barrelEntity = this.Owner.FindChild(GameConstants.EntitynameTankBarrel);
            this.barrel = barrelEntity.FindComponent<Sprite>();
            this.barrelTransform = barrelEntity.FindComponent<Transform2D>();

            this.body = this.Owner.FindChild(GameConstants.EntityNameTankBody).FindComponent<Sprite>();

            this.CurrentLive = this.InitialLive;
            this.CurrentSpeed = this.InitialSpeed;
            this.CurrentRotationSpeed = this.InitialRotationSpeed;
            this.CurrentRotationBarrelSpeed = this.InitialRotationBarrelSpeed;
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

        internal void Move(float forward, float elapsedTime)
        {
            var orientation = this.transform.Orientation;
            this.transform.LocalPosition += forward * (orientation * Vector3.UnitY * elapsedTime * this.CurrentSpeed).ToVector2();
        }

        internal void Rotate(float left, float elapsedTime)
        {
            var roll = left * this.CurrentRotationSpeed * elapsedTime;
            this.transform.Orientation = this.transform.Orientation * Quaternion.CreateFromYawPitchRoll(0.0f, 0.0f, roll);
        }

        internal void RotateBarrel(float left, float elapsedTime)
        {
            var roll = left * this.CurrentRotationBarrelSpeed * elapsedTime;
            this.barrelTransform.Orientation = this.barrelTransform.Orientation * Quaternion.CreateFromYawPitchRoll(0.0f, 0.0f, roll);
        }
    }
}
