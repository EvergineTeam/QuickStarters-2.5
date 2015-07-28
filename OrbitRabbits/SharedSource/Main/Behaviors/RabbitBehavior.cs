#region File Description
//-----------------------------------------------------------------------------
// OrbitRabbits
//
// Quickstarter for Wave University Tour 2014.
// Author: Wave Engine Team
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using OrbitRabbits.Commons;
using OrbitRabbits.Components;
using OrbitRabbits.Entities;
using OrbitRabbits.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Components.Particles;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
#endregion

namespace OrbitRabbits.Behaviors
{
    [DataContract(Namespace = "OrbitRabbits.Behaviors")]
    public class RabbitBehavior : Behavior
    {
        private Vector2 moonPosition = new Vector2(382, 605);
        private float moonRadio = 130;
        private float gravityRadio = 400;
        private float moonForce = 15f;
        private float moonMass = 9;

        private Vector2 position;
        private float mass = 4;
        private Vector2 velocity;
        private Vector2 initialDirection;
        private Rabbit.RabbitState state;
        private TimeSpan stillTime;
        private ParticleSystem2D rabbitParticles;        

        [RequiredComponent]
        private Transform2D transform = null;

        [RequiredComponent]
        private SpriteAtlas sprite = null;

        #region Properties

        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        public Rabbit.RabbitState State
        {
            get { return state; }
            set
            {
                state = value;
                switch (state)
                {
                    case Rabbit.RabbitState.still:
                        if (this.rabbitParticles != null) this.rabbitParticles.Emit = false;
                        break;
                    case Rabbit.RabbitState.afloat:
                        if (this.rabbitParticles != null) this.rabbitParticles.Emit = true;
                        break;

                    case Rabbit.RabbitState.dying:
                        this.velocity = Vector2.Zero;
                        this.sprite.TintColor = Color.Gray;
                        if (this.rabbitParticles != null) this.rabbitParticles.Emit = false;
                        SoundsManager.Instance.PlaySound(SoundsManager.SOUNDS.Fall);
                        break;
                    case Rabbit.RabbitState.dead:
                        if (this.rabbitParticles != null) this.rabbitParticles.Emit = false;
                        break;
                }
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="RabbitBehavior" /> class.
        /// </summary>
        public RabbitBehavior()
        {
            this.velocity = new Vector2(0.2f, 0);
            this.initialDirection = Vector2.UnitX;
            this.State = Rabbit.RabbitState.still;
            this.stillTime = TimeSpan.Zero;
        }

        /// <summary>
        /// Resolves the dependencies needed for this instance to work.
        /// </summary>
        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            this.rabbitParticles = this.Owner.FindChild("rabbitParticles").FindComponent<ParticleSystem2D>();            
        }

        /// <summary>
        /// Allows this instance to execute custom logic during its <c>Update</c>.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        /// <remarks>
        /// This method will not be executed if the <see cref="T:WaveEngine.Framework.Component" />, or the <see cref="T:WaveEngine.Framework.Entity" />
        /// owning it are not <c>Active</c>.
        /// </remarks>
        protected override void Update(TimeSpan gameTime)
        {
            // Position
            this.position.X = transform.X;
            this.position.Y = transform.Y;

            Vector2 distance = this.moonPosition - this.position;
            float distanceSquare = distance.LengthSquared();

            // CheckCollision            
            float force = this.moonForce * ((this.moonMass * this.mass) / distanceSquare);

            distance.Normalize();

            this.velocity += distance * force;

            this.position += this.velocity * (float)gameTime.TotalMilliseconds;

            Vector2 newDistance = this.moonPosition - this.position;
            float newDistanceLength = newDistance.Length();
            if (newDistanceLength < this.moonRadio)
            {
                this.velocity = Vector2.Zero;

                switch (this.state)
                {
                    case Rabbit.RabbitState.afloat:
                        this.State = Rabbit.RabbitState.still;
                        break;
                    case Rabbit.RabbitState.dying:
                        this.State = Rabbit.RabbitState.dead;
                        break;
                }
            }
            else if (newDistanceLength > this.gravityRadio)
            {
                this.State = Rabbit.RabbitState.dying;
            }
            else
            {
                this.transform.X = this.position.X;
                this.transform.Y = this.position.Y;
            }

            // Rotation
            if (this.state == Rabbit.RabbitState.dying)
            {
                this.transform.Rotation += (float)gameTime.TotalMilliseconds / 250;
            }
            else
            {
                float angle = Vector2.Angle(this.initialDirection, distance);
                this.transform.Rotation = -angle;
            }

            // Still Time
            if (this.state == Rabbit.RabbitState.still)
            {
                this.stillTime += gameTime;
                if (this.stillTime > TimeSpan.FromSeconds(3))
                {
                    this.State = Rabbit.RabbitState.dead;
                }
            }
            else
            {
                this.stillTime = TimeSpan.Zero;
            }
        }

        /// <summary>
        /// Applies the impulse.
        /// </summary>
        public void ApplyImpulse()
        {
            if (this.state == Rabbit.RabbitState.dead ||
                this.state == Rabbit.RabbitState.dying)
            {
                return;
            }

            // Position
            this.position.X = transform.X;
            this.position.Y = transform.Y;

            Vector2 direction = this.position - this.moonPosition;

            float distanceLenghtSquare = direction.LengthSquared();
            direction.Normalize();
                        
            //this.velocity += Utils.RotateVectorAroundPoint(direction * 5000 / distanceLenghtSquare, this.moonPosition, 30);
            this.velocity += Utils.RotateVectorAroundPoint(direction * 5000 / distanceLenghtSquare, this.moonPosition, 30);

            this.State = Rabbit.RabbitState.afloat;
        }
    }
}
