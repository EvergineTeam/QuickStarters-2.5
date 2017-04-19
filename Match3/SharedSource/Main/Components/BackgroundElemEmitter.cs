using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using WaveEngine.Common.Math;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Components.Graphics3D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;

namespace Match3.Components
{
    [DataContract]
    public class BackgroundElemEmitter : Behavior
    {
        private const int PoolSize = 150;

        [RequiredService]
        protected WaveEngine.Framework.Services.Random randomService;

        [DataMember]
        public int ElementRadius { get; set; }

        [DataMember]
        public int ElementsPadding { get; set; }

        [DataMember]
        public Vector2 LinearVelocity { get; set; }

        [DataMember]
        public float MaxRotationVelocity { get; set; }

        [DataMember]
        public float MinRotationVelocity { get; set; }

        private TimeSpan apparitionInterval;

        private TimeSpan lastAparitionCounter;

        private List<Transform2D> availableEntities;

        private List<Transform2D> onSceenEntities;

        private Vector2 startPoint, endPoint;

        private float numElements;

        private float maxElementDistance;

        protected override void DefaultValues()
        {
            base.DefaultValues();

            this.lastAparitionCounter = TimeSpan.Zero;
            this.availableEntities = new List<Transform2D>(PoolSize);
            this.onSceenEntities = new List<Transform2D>();

            this.ElementRadius = 90;
            this.ElementsPadding = 20;
            this.LinearVelocity = new Vector2(3);
            this.MinRotationVelocity = 0;
            this.MaxRotationVelocity = 2;
        }

        protected override void Initialize()
        {
            base.Initialize();

            this.FillPool();

            this.CalculateGenerationParameters();

            this.Owner.EntityInitialized += this.Owner_EntityInitialized;
        }

        private void Owner_EntityInitialized(object sender, EventArgs e)
        {
            this.Owner.EntityInitialized -= this.Owner_EntityInitialized;
            this.RefillScreen();
        }

        private void CalculateGenerationParameters()
        {
            this.apparitionInterval = TimeSpan.FromSeconds((this.ElementRadius + this.ElementsPadding) / Math.Min(this.LinearVelocity.X, this.LinearVelocity.Y));

            var virtualScreen = this.Owner.Scene.VirtualScreenManager;
            var origin = new Vector2(virtualScreen.LeftEdge, virtualScreen.TopEdge) - new Vector2(this.ElementRadius);
            var rightBottomCorner = new Vector2(virtualScreen.RightEdge, virtualScreen.BottomEdge) + new Vector2(this.ElementRadius);
            var rightCorner = new Vector2(virtualScreen.RightEdge, origin.Y);
            var bottomCorner = new Vector2(origin.X, virtualScreen.BottomEdge);

            var perperdicular = new Vector2(this.LinearVelocity.Y, -this.LinearVelocity.X);
            perperdicular.Normalize();
            this.endPoint = (Vector2.Dot(rightCorner - origin, perperdicular) * perperdicular) + origin;
            this.startPoint = (Vector2.Dot(bottomCorner - origin, perperdicular) * perperdicular) + origin;
            
            this.maxElementDistance = (rightBottomCorner - origin).Length();

            this.numElements = (int)((this.endPoint - this.startPoint).Length() / (2f * (this.ElementsPadding + this.ElementRadius)));
        }

        private void FillPool()
        {
            for (int i = 0; i < PoolSize; i++)
            {
                var elementTransform = new Transform2D()
                {
                    Origin = Vector2.Center
                };

                var elementEntity = new Entity()
                {
                    IsSerializable = false
                }
                    .AddComponent(elementTransform)
                    .AddComponent(new Spinner())
                    .AddComponent(new SpriteAtlas()
                    {
                        SpriteSheetPath = WaveContent.Assets.GUI.BackgroundElems_spritesheet
                    })
                    .AddComponent(new SpriteAtlasRenderer());

                this.availableEntities.Add(elementTransform);
            }
        }

        protected override void Update(TimeSpan gameTime)
        {
            var deltaTime = (float)gameTime.TotalSeconds;

            foreach (var elementTransform in this.onSceenEntities.ToList())
            {
                elementTransform.Position += this.LinearVelocity * deltaTime;

                if (this.IsOutOfScreen(elementTransform.Position))
                {
                    this.RemoveElementFromScreen(elementTransform);
                }
            }

            this.lastAparitionCounter += gameTime;
            if (this.lastAparitionCounter >= this.apparitionInterval)
            {
                this.lastAparitionCounter = TimeSpan.Zero;

                var distanceAmount = 1f / (this.numElements - 1);
                var rndPosition = (float)randomService.NextDouble() * distanceAmount;
                for (int i = 0; i < this.numElements; i++)
                {
                    var position = Vector2.Lerp(this.startPoint, this.endPoint, rndPosition);
                    rndPosition += distanceAmount;
                    this.AddElementFromPool(position);
                }
            }
        }

        private void RefillScreen()
        {
            // Remove all existing elements
            foreach (var elementTransform in this.onSceenEntities.ToList())
            {
                this.RemoveElementFromScreen(elementTransform);
            }

            // Fill screen with elements
            var maxAdvanceVelocity = Math.Max(this.LinearVelocity.X, this.LinearVelocity.Y);
            float totalSecondsAmount = this.maxElementDistance / maxAdvanceVelocity;
            var steps = Math.Ceiling(totalSecondsAmount / this.apparitionInterval.TotalSeconds);

            for (int i = 0; i < steps; i++)
            {
                this.Update(this.apparitionInterval);
            }
        }

        private bool IsOutOfScreen(Vector2 position)
        {
            var virtualScreen = this.Owner.Scene.VirtualScreenManager;

            return position.X >= (virtualScreen.RightEdge + this.ElementRadius) ||
                   position.Y >= (virtualScreen.BottomEdge + this.ElementRadius);
        }

        private void AddElementFromPool(Vector2 position)
        {
            if (this.availableEntities.Count <= 0 ||
                this.IsOutOfScreen(position))
            {
                return;
            }

            var elementEntity = this.availableEntities.First();
            this.availableEntities.Remove(elementEntity);
            this.onSceenEntities.Add(elementEntity);
            this.Owner.AddChild(elementEntity.Owner);

            var spriteAtlas = elementEntity.Owner.FindComponent<SpriteAtlas>();
            var rndTexture = randomService.Next(spriteAtlas.TextureNames.Count());
            spriteAtlas.TextureName = spriteAtlas.TextureNames.ElementAt(rndTexture);

            var spinner = elementEntity.Owner.FindComponent<Spinner>();
            var rndRotation = this.MinRotationVelocity + (this.MaxRotationVelocity - this.MinRotationVelocity) * (float)randomService.NextDouble();
            spinner.IncreaseZ = rndRotation;

            var virtualScreen = this.Owner.Scene.VirtualScreenManager;
            elementEntity.Position = position;
        }

        private void RemoveElementFromScreen(Transform2D tranform)
        {
            this.onSceenEntities.Remove(tranform);
            this.availableEntities.Add(tranform);

            this.Owner.DetachChild(tranform.Owner.Name);
        }
    }
}
