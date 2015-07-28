#region File Description
//-----------------------------------------------------------------------------
// OrbitRabbits
//
// Quickstarter for Wave University Tour 2014.
// Author: Wave Engine Team
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using OrbitRabbits.Behaviors;
using OrbitRabbits.Commons;
using OrbitRabbits.Entities.Behaviors;
using OrbitRabbits.Entities.Particles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Math;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Components.Graphics3D;
using WaveEngine.Components.Particles;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Models;
using WaveEngine.Framework.Physics2D;
using WaveEngine.Framework.Services;
using WaveEngine.Materials;
#endregion

namespace OrbitRabbits.Entities
{
    public class Rabbit : BaseDecorator
    {
        public enum RabbitState
        {
            still,
            afloat,
            dying,
            dead
        };

        private RabbitBehavior rabbitBehavior;
        private RectangleCollider2D collider;

        #region Properties
        public RabbitState State
        {
            get { return this.rabbitBehavior.State; }
            set { this.rabbitBehavior.State = value; }
        }

        public Transform2D Transform2D
        {
            get { return this.entity.FindComponent<Transform2D>(); }
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Rabbit" /> class.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="drawOrder">The draw order.</param>
        /// <param name="assetsContainer">The assets container</param>
        public Rabbit(Vector2 position, float drawOrder, AssetsContainer assetsContainer)
        {
            this.entity = new Entity()
                            .AddComponent(new Transform2D()
                            {
                                Origin = Vector2.Center,
                                X = position.X,
                                Y = position.Y,
                                DrawOrder = drawOrder,
                                XScale = 0.8f,
                                YScale = 0.8f,
                            })
                            .AddComponent(new RectangleCollider2D())
                            .AddComponent(new RabbitBehavior())
                            .AddComponent(new SpriteAtlas(Directories.TexturePath + "game.png", "rabbit"))
                            .AddComponent(new SpriteAtlasRenderer(DefaultLayers.Alpha));

            // Cached            
            this.rabbitBehavior = this.entity.FindComponent<RabbitBehavior>();
            this.collider = this.entity.FindComponent<RectangleCollider2D>();
             
            var materialModel = assetsContainer.LoadModel<MaterialModel>(Directories.MaterialPath + "StarParticleMaterial.wmat");

            // Particles
            this.entity.AddChild(new Entity("rabbitParticles")
                                        .AddComponent(new Transform2D())
                                        .AddComponent(ParticleFactory.CreateStarsParticle())
                                        .AddComponent(new MaterialsMap(materialModel.Material))
                                        .AddComponent(new ParticleSystemRenderer2D("rabbitParticles")));
        }

        /// <summary>
        /// Collisions the specified rabbit.
        /// </summary>
        /// <param name="rabbit">The rabbit.</param>
        /// <returns>true if collision</returns>
        public bool Collision(Rabbit rabbit)
        {
            return this.collider.Intersects(rabbit.collider);
        }

        /// <summary>
        /// Applies the impulse.
        /// </summary>
        public void ApplyImpulse()
        {
            this.rabbitBehavior.ApplyImpulse();           
        }
    }
}
