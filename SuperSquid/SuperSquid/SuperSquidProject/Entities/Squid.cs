#region File Description
//-----------------------------------------------------------------------------
// Super Squid
//
// Quickstarter for Wave University Tour 2014.
// Author: Wave Engine Team
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using SuperSquidProject.Commons;
using SuperSquidProject.Entities.Behaviors;
using SuperSquidProject.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Math;
using WaveEngine.Components.Animation;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Components.Particles;
using WaveEngine.Framework;
using WaveEngine.Framework.Animation;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Physics2D;
using WaveEngine.Framework.Services;
using WaveEngine.Materials;
#endregion

namespace SuperSquidProject.Entities
{
    public class Squid : BaseDecorator
    {
        private Animation2D animation2D;
        private float initialPosY;
        private float gamePlayPosY;
        private Vector2 direction;
        private Transform2D transform;
        private ParticleSystem2D particleSystem;
        private AnimationUI animation;
        private SingleAnimation appearAnim;

        /// <summary>
        /// Initializes a new instance of the <see cref="Squid" /> class.
        /// </summary>
        /// <param name="positionY">The position Y.</param>
        public Squid(float positionY)
        {
            this.initialPosY = positionY;
            this.gamePlayPosY = WaveServices.ViewportManager.BottomEdge + 50;

            this.entity = new Entity("SquidEntity")
                            .AddComponent(new Transform2D()
                            {
                                Origin = new Vector2(0.5f, 0f),
                                X = WaveServices.ViewportManager.VirtualWidth / 2,
                                Y = this.gamePlayPosY,
                                DrawOrder = 0.3f,
                            })
                            .AddComponent(new AnimationUI())
                            .AddComponent(new SquidBehavior())
                            .AddComponent(new Sprite(Directories.TexturePath + "squidSpriteSheet.wpk"))
                            .AddComponent(Animation2D.Create<TexturePackerGenericXml>(Directories.TexturePath + "squidSpriteSheet.xml")
                                                .Add("swim", new SpriteSheetAnimationSequence() { First = 1, Length = 30, FramesPerSecond = 30 }))
                            .AddComponent(new PerPixelCollider(Directories.TexturePath + "squidCollider.wpk", 0.5f))
                            .AddComponent(new AnimatedSpriteRenderer(DefaultLayers.Alpha));

            // Cached
            this.transform = this.entity.FindComponent<Transform2D>();
            this.animation2D = this.entity.FindComponent<Animation2D>();
            this.direction = -Vector2.UnitY;
            this.animation = this.entity.FindComponent<AnimationUI>();

            // Bubble
            this.entity.AddChild(new Entity("bubblesParticle")
                                    .AddComponent(new Transform2D()
                                    {
                                        Y = 210,
                                    })
                                    .AddComponent(ParticleFactory.CreateBubbleParticles())
                                    .AddComponent(new Material2D(new BasicMaterial2D(Directories.TexturePath + "waterParticle.wpk", DefaultLayers.Additive)))
                                    .AddComponent(new ParticleSystemRenderer2D("bubblesParticle"))
                                    .AddComponent(new ChildFollower2D()));

            // Cached
            this.particleSystem = this.entity.FindChild("bubblesParticle").FindComponent<ParticleSystem2D>();

            // Animations
            this.appearAnim = new SingleAnimation(gamePlayPosY, this.initialPosY, TimeSpan.FromSeconds(1.5f), EasingFunctions.Cubic);            
        }

        /// <summary>
        /// Appears this instance.
        /// </summary>
        public void Appear()
        {
            this.transform.X = WaveServices.ViewportManager.VirtualWidth / 2;
            this.animation.BeginAnimation(Transform2D.YProperty, this.appearAnim);
            this.animation2D.Play(true);            
        }

        /// <summary>
        /// Apply impulse to squid
        /// </summary>
        //public void Impulse()
        //{
        //    Vector2 position = new Vector2(transform.X, transform.Y);
        //    this.animation2D.Play(false);
        //    this.LaunchBubbles();
        //}

        /// <summary>
        /// Lanches the bubbles.
        /// </summary>
        //private void LaunchBubbles()
        //{
        //    this.particleSystem.Emit = true;
        //    WaveServices.TimerFactory.CreateTimer("bubbles", TimeSpan.FromSeconds(0.5f), () =>
        //    {
        //        this.particleSystem.Emit = false;
        //    }, false);
        //}
    }
}
