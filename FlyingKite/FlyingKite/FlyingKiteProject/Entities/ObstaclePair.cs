#region File Description
//-----------------------------------------------------------------------------
// Flying Kite
//
// Quickstarter for Wave University Tour 2014.
// Author: Wave Engine Team
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using FlyingKiteProject.Behaviors;
using FlyingKiteProject.Components;
using FlyingKiteProject.Layers;
using FlyingKiteProject.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Components.Particles;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Physics2D;
using WaveEngine.Framework.Services;
using WaveEngine.Materials;
#endregion

namespace FlyingKiteProject.Entities
{
    public class ObstaclePair : BaseDecorator
    {
        private const int OBSTACLES_SEPARATION = 225;

        private static int instancesCount;

        private Entity starEntity, starParticles;

        #region Properties
        public Transform2D Transform2D
        {
            get;
            private set;
        }

        public Collider2D TopCollider
        {
            get;
            private set;
        }

        public Collider2D BottomCollider
        {
            get;
            private set;
        }

        public Collider2D StarCollider
        {
            get;
            private set;
        }

        public bool StarAvaible
        {
            get
            {
                return this.starEntity.IsVisible;
            }

            set
            {
                this.starEntity.IsVisible = value;
            }
        } 
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ObstaclePair" /> class.
        /// </summary>
        /// <param name="reappearanceX">The reappearance X.</param>
        public ObstaclePair(float reappearanceX)
        {
            var topObstacleY = (WaveServices.ViewportManager.VirtualHeight - OBSTACLES_SEPARATION) / 2;

            var topObstacle = new Entity("TopObstacle")
                                .AddComponent(new Transform2D()
                                {
                                    Y = topObstacleY,
                                    Origin = Vector2.UnitY
                                })
                                .AddComponent(new SpriteAtlas(Textures.GAME_ATLAS, Textures.GameAtlas.obstacle_top.ToString()))
                                .AddComponent(new SpriteAtlasRenderer(typeof(ObstaclesLayer)))
                                .AddComponent(this.TopCollider = new PerPixelCollider(Textures.OBSTACLE_TOP_COLLID, 0.5f));

            var bottomObstacle = new Entity("BottomObstacle")
                                .AddComponent(new Transform2D()
                                {
                                    X = 75,
                                    Y = topObstacleY + OBSTACLES_SEPARATION
                                })
                                .AddComponent(new SpriteAtlas(Textures.GAME_ATLAS, Textures.GameAtlas.obstacle_bottom.ToString()))
                                .AddComponent(new SpriteAtlasRenderer(typeof(ObstaclesLayer)))
                                .AddComponent(this.BottomCollider = new PerPixelCollider(Textures.OBSTACLE_BOTTOM_COLLID, 0.5f));

            this.starEntity = EntitiesFactory.CreateGameStar(
                0,
                WaveServices.ViewportManager.VirtualHeight / 2)
                .AddComponent(this.StarCollider = new CircleCollider());

            this.starParticles = new Entity()
                .AddComponent(new Transform2D()
                {
                    X = 0,
                    Y = WaveServices.ViewportManager.VirtualHeight / 2,
                })
                .AddComponent(new Material2D(new BasicMaterial2D(Textures.STAR, DefaultLayers.Alpha)))
                .AddComponent(new ParticleSystemRenderer2D("explosion"))
                .AddComponent(new ParticleSystem2D()
                {
                    NumParticles = 20,
                    EmitRate = 300,
                    MinLife = TimeSpan.FromSeconds(3f),
                    MaxLife = TimeSpan.FromSeconds(1f),
                    LocalVelocity = new Vector2(-3.5f, -5f),
                    RandomVelocity = new Vector2(3.5f, 1f),
                    MinSize = 5,
                    MaxSize = 20,
                    MinRotateSpeed = 0.03f,
                    MaxRotateSpeed = -0.03f,
                    EndDeltaScale = 0f,
                    LinearColorEnabled = true,
                    InterpolationColors = new List<Color>() { Color.White, Color.Black},
                    EmitterSize = new Vector3(30,30,0),
                    Gravity = Vector2.UnitY * 0.2f,
                    EmitterShape = ParticleSystem2D.Shape.FillCircle,
                    Emit = false,
                });


            var scrollBehavior = new ScrollBehavior(1);

            this.entity = new Entity("ObstaclePair" + instancesCount++)
                       {
                           Tag = "OBSTACLE"
                       }
                       .AddComponent(this.Transform2D = new Transform2D()
                       {
                           X = WaveServices.ViewportManager.RightEdge,
                           DrawOrder = 0.8f
                       })
                       .AddComponent(scrollBehavior)
                       .AddChild(topObstacle)
                       .AddChild(bottomObstacle)
                       .AddChild(this.starEntity)
                       .AddChild(this.starParticles)
                       .AddComponent(new ChildrenRectangle());

            scrollBehavior.EntityOutOfScreen += (entity) =>
            {
                this.Transform2D.X += reappearanceX;
                this.Transform2D.Y = this.GetRandomY();
                this.StarAvaible = true;
            };
        }

        /// <summary>
        /// Shots the star particles.
        /// </summary>
        public void ShotStarParticles()
        {
            var particleSystem = this.starParticles.FindComponent<ParticleSystem2D>();

            particleSystem.Emit = true;
            WaveServices.TimerFactory.CreateTimer("explosionTimer", TimeSpan.FromSeconds(0.6f), () =>
            {
                particleSystem.Emit = false;
            }, false);
        }

        /// <summary>
        /// Gets a new random Y position.
        /// </summary>
        /// <returns></returns>
        private float GetRandomY()
        {
            var randomOffsetY = (int)(this.Transform2D.Rectangle.Height - WaveServices.ViewportManager.VirtualHeight) / 2;
            return WaveServices.Random.Next(-randomOffsetY, randomOffsetY);
        }
    }
}
