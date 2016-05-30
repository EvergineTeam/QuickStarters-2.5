using System;
using System.Runtime.Serialization;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Managers;
using WaveEngine.Framework.Services;

namespace DeepSpace.Components.Stars
{
    [DataContract]
    public class StarBehavior : Behavior
    {
        [RequiredComponent]
        protected Transform2D transform;

        [RequiredComponent]
        protected Sprite sprite;
        private VirtualScreenManager virtualScreenManager;
        private WaveEngine.Framework.Services.Random randomService;

        [DataMember]
        public float Speed { get; set; }

        [DataMember]
        public float Margin { get; set; }

        public StarBehavior()
            : base()
        {
        }

        protected override void DefaultValues()
        {
            base.DefaultValues();
        }

        protected override void Initialize()
        {
            base.Initialize();
            this.virtualScreenManager = this.Owner.Scene.VirtualScreenManager;
            this.randomService = WaveServices.Random;
        }

        protected override void Update(TimeSpan gameTime)
        {
            float time = (float)gameTime.TotalMilliseconds / 10f;

            var displace = this.Speed * time;
            this.transform.Y += displace;

            this.transform.YScale = ((displace * 0.5f) + this.sprite.Texture.Height) / this.sprite.Texture.Height;

            if (this.transform.Y > this.virtualScreenManager.BottomEdge + this.Margin)
            {
                this.transform.Y = this.virtualScreenManager.TopEdge - this.Margin;
                this.transform.X = (float)this.randomService.NextDouble() * this.virtualScreenManager.VirtualWidth;
            }
        }
    }
}
