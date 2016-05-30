using System;
using System.Runtime.Serialization;
using WaveEngine.Common.Graphics;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;

namespace DeepSpace.Components.Stars
{
    [DataContract]
    public class StarsGeneratorBehavior : Behavior
    {
        private bool isStarsGenerated;

        public StarsGeneratorBehavior() : base("StarsGeneratorBehavior")
        {
        }

        protected override void DefaultValues()
        {
            base.DefaultValues();
        }

        protected override void Update(TimeSpan gameTime)
        {
            if (!this.isStarsGenerated)
            {
                this.GenerateStars();
            }
        }

        private void GenerateStars()
        {
            var virtualScreenManager = this.Owner.Scene.VirtualScreenManager;

            for (int i = 0; i < 100; i++)
            {
                Entity star = new Entity()
                    .AddComponent(new Transform2D()
                    {
                        X = WaveServices.Random.Next((int)virtualScreenManager.VirtualWidth),
                        Y = WaveServices.Random.Next((int)virtualScreenManager.VirtualHeight),
                        DrawOrder = 0.8f
                    })
                    .AddComponent(new Sprite(WaveContent.Assets.Star_jpg)
                    {
                        TintColor = new Color((float)WaveServices.Random.NextDouble() * 0.3f, (float)WaveServices.Random.NextDouble() * 0.3f, (float)WaveServices.Random.NextDouble() * 0.3f)
                    })
                    .AddComponent(new SpriteRenderer(DefaultLayers.Additive))
                    .AddComponent(new StarBehavior { Speed = (float)WaveServices.Random.NextDouble() * 10, Margin = 0 });

                EntityManager.Add(star);
            }

            this.isStarsGenerated = true;
        }
    }
}
