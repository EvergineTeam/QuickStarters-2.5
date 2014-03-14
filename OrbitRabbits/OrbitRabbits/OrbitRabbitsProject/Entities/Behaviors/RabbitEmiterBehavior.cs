#region File Description
//-----------------------------------------------------------------------------
// OrbitRabbits
//
// Quickstarter for Wave University Tour 2014.
// Author: Wave Engine Team
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using OrbitRabbitsProject.Commons;
using OrbitRabbitsProject.Entities.Particles;
using OrbitRabbitsProject.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Math;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Components.Particles;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
using WaveEngine.Materials;
#endregion

namespace OrbitRabbitsProject.Entities.Behaviors
{
    public class RabbitEmiterBehavior : Behavior
    {
        private Vector2 initialRabbitPosition = new Vector2(375, 460);
        private List<Rabbit> rabbits;
        private List<Rabbit> deadRabbits;
        private Rabbit lastRabbit;
        private Entity explosion;
        private Transform2D explosionTransform;
        private ParticleSystem2D explosionSystem;

        public event EventHandler<int> scoreChanged;

        private TimeSpan time;

        /// <summary>
        /// Initializes a new instance of the <see cref="RabbitEmiterBehavior" /> class.
        /// </summary>
        /// <param name="rabbits">The rabbits.</param>
        public RabbitEmiterBehavior(List<Rabbit> rabbits)
        {
            this.rabbits = rabbits;
            this.deadRabbits = new List<Rabbit>();

            // Explosion
            this.explosion = new Entity("explosion")
                                .AddComponent(new Transform2D())
                                .AddComponent(ParticleFactory.CreateExplosion())
                                .AddComponent(new Material2D(new BasicMaterial2D(Directories.TexturePath + "starParticle.wpk")))
                                .AddComponent(new ParticleSystemRenderer2D("explosion", DefaultLayers.Additive));  
          
            // Cached
            this.explosionSystem = this.explosion.FindComponent<ParticleSystem2D>();
            this.explosionTransform = this.explosion.FindComponent<Transform2D>();
        }

        /// <summary>
        /// Resolves the dependencies needed for this instance to work.
        /// </summary>
        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            this.Owner.AddChild(this.explosion);
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
            bool updateScore = false;

            this.time += gameTime;

            if ( (this.lastRabbit != null && (this.lastRabbit.State == Rabbit.RabbitState.dying ||
                this.lastRabbit.State == Rabbit.RabbitState.dead)) ||
                this.time > TimeSpan.FromSeconds(5))
            {
                this.AddRabbit();
                this.time = TimeSpan.Zero;
            }

            // Clear rabbit deads
            foreach (Rabbit rabbit in this.rabbits)
            {
                if (rabbit.State == Rabbit.RabbitState.dead)
                {
                    this.deadRabbits.Add(rabbit);                    
                }
            }

            if (this.deadRabbits.Count > 0)
            {
                foreach (Rabbit deadRabbit in deadRabbits)
                {
                    this.CreateExplosion(deadRabbit);
                    this.Owner.RemoveChild(deadRabbit.Name);
                    this.rabbits.Remove(deadRabbit);                    
                }
                this.deadRabbits.Clear();

                updateScore = true;
            }

            // Check Collision
            for (int i = 0; i < this.rabbits.Count - 1; i++)
            {
                Rabbit rabbit1 = this.rabbits[i];
                if (rabbit1.State != Rabbit.RabbitState.afloat) { continue; }

                for (int j = i + 1; j < this.rabbits.Count; j++)
                {
                    Rabbit rabbit2 = this.rabbits[j];
                    if (rabbit2.State != Rabbit.RabbitState.afloat) { continue; }

                    bool collision = rabbit1.Collision(rabbit2);
                    if (collision)
                    {
                        rabbit1.State = Rabbit.RabbitState.dead;
                        rabbit2.State = Rabbit.RabbitState.dead;
                        updateScore = true;

                        this.CreateExplosion(rabbit1, rabbit2);
                    }
                }
            }

            // Update Scores
            if (updateScore)
            {
                this.NotifyNewScore();
            }
        }

        /// <summary>
        /// Adds the rabbit.
        /// </summary>
        public void AddRabbit()
        {
            Rabbit newRabbit = new Rabbit(this.initialRabbitPosition, 0.75f);
            this.Owner.AddChild(newRabbit.Entity);
            this.rabbits.Add(newRabbit);

            this.lastRabbit = newRabbit;

            this.NotifyNewScore();
        }

        /// <summary>
        /// Notifies the new score.
        /// </summary>
        private void NotifyNewScore()
        {
            if (this.scoreChanged != null)
            {
                int rabbitAliveCount = (from rabbit in this.rabbits
                                        where rabbit.State == Rabbit.RabbitState.afloat
                                        select rabbit).Count();
                this.scoreChanged(this, rabbitAliveCount);
            }
        }

        /// <summary>
        /// Applies the impulse.
        /// </summary>
        public void ApplyImpulse()
        {
            if (this.lastRabbit != null)
            {
                this.time = TimeSpan.Zero;
                this.lastRabbit.ApplyImpulse();

                SoundsManager.Instance.PlaySound(SoundsManager.SOUNDS.Impulse);
            }
        }

        /// <summary>
        /// Creates the explosion.
        /// </summary>
        /// <param name="rabbit1">The rabbit1.</param>
        /// <param name="rabbit2">The rabbit2.</param>
        private void CreateExplosion(Rabbit rabbit1, Rabbit rabbit2)
        {
            Vector2 rabbit1Position = new Vector2(rabbit1.Transform2D.X, rabbit1.Transform2D.Y);
            Vector2 rabbit2Position = new Vector2(rabbit2.Transform2D.X, rabbit2.Transform2D.Y);

            Vector2 distance = rabbit2Position - rabbit1Position;
            float length = distance.Length();
            distance.Normalize();

            Vector2 explosionPosition = rabbit1Position + distance * length / 2;
            this.explosionTransform.X = explosionPosition.X;
            this.explosionTransform.Y = explosionPosition.Y;

            this.explosionSystem.Emit = true;
            WaveServices.TimerFactory.CreateTimer("explosionTimer", TimeSpan.FromSeconds(0.6f), () =>
            {
                this.explosionSystem.Emit = false;
            },false);

            SoundsManager.Instance.PlaySound(SoundsManager.SOUNDS.Explosion);
        }

        /// <summary>
        /// Creates the explosion.
        /// </summary>
        /// <param name="rabbit1">The rabbit1.</param>
        private void CreateExplosion(Rabbit rabbit1)
        {
            Vector2 explosionPosition = new Vector2(rabbit1.Transform2D.X, rabbit1.Transform2D.Y);
            this.explosionTransform.X = explosionPosition.X;
            this.explosionTransform.Y = explosionPosition.Y;

            this.explosionSystem.Emit = true;
            WaveServices.TimerFactory.CreateTimer("explosionTimer", TimeSpan.FromSeconds(1), () =>
            {
                this.explosionSystem.Emit = false;
            }, false);

            SoundsManager.Instance.PlaySound(SoundsManager.SOUNDS.Explosion);
        }
    }
}
