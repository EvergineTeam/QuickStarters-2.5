#region Using Statements
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using SuperSlingshot.Behaviors;
using SuperSlingshot.Components;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Math;
using WaveEngine.Common.Physics2D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Physics2D;
using WaveEngine.Framework.Services;
using WaveEngine.ImageEffects;
using WaveEngine.TiledMap;
#endregion

namespace SuperSlingshot.Behaviors
{
    [DataContract]
    public class AutoMoveBehavior : Behavior
    {
        private Vector2 currentSpeed;

        public enum Cycle
        {
            None,
            Horizontal,
            Vertical,
            Both
        }

        [RequiredComponent]
        private Transform2D transform = null;

        [DataMember]
        [RenderPropertyAsVector2Input]
        public Vector2 MaxSpeed { get; set; }

        [DataMember]
        [RenderPropertyAsVector2Input]
        public Vector2 MinSpeed { get; set; }

        [DataMember]
        public Cycle CicleDirection { get; set; }

        [DataMember]
        public float MinX { get; set; }

        [DataMember]
        public float MaxX { get; set; }

        [DataMember]
        public float MinY { get; set; }

        [DataMember]
        public float MaxY { get; set; }

        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            var speedDiff = this.MaxSpeed - this.MinSpeed;
            this.currentSpeed = this.MinSpeed + (speedDiff * (float)WaveServices.FastRandom.NextDouble());
        }

        protected override void Update(TimeSpan gameTime)
        {
            var position = this.transform.LocalPosition;
            var delta = this.currentSpeed * (float)gameTime.TotalSeconds;

            if (this.CicleDirection == Cycle.Horizontal
               || this.CicleDirection == Cycle.Both)
            {
                position.X += delta.X;
                if (position.X > this.MaxX)
                {
                    position.X = this.MinX;
                }
                else if (position.X < this.MinX)
                {
                    position.X = this.MaxX;
                }
            }

            if (this.CicleDirection == Cycle.Vertical
               || this.CicleDirection == Cycle.Both)
            {
                position.Y += delta.Y;
                if (position.Y > this.MaxY)
                {
                    position.Y = this.MinY;
                }
                else if (position.Y < this.MinY)
                {
                    position.Y = this.MaxY;
                }
            }

            this.transform.LocalPosition = position;
        }
    }
}
