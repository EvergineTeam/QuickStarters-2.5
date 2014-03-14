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
using FlyingKiteProject.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Math;
using WaveEngine.Components.Animation;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Physics2D;
using WaveEngine.Framework.Services;
#endregion

namespace FlyingKiteProject.Entities
{
    public class Kite : BaseDecorator
    {
        private readonly string[] kiteAnimations = 
        {
            "Blue",
            "Green",
            "Orange"
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="Kite" /> class.
        /// </summary>
        public Kite()
        {
            this.entity = new Entity("kite")
                        .AddComponent(new Transform2D()
                        {
                            X = WaveServices.ViewportManager.VirtualWidth / 4,
                            Y = WaveServices.ViewportManager.VirtualHeight / 2,
                            Origin = Vector2.Center
                        })
                        .AddComponent(new Sprite(Textures.KITE_ANIMS))
                        .AddComponent(Animation2D.Create<TexturePackerGenericXml>(Textures.KITE_ANIMS_XML)
                            .Add(this.kiteAnimations[0], new SpriteSheetAnimationSequence() { First = 1, Length = 9, FramesPerSecond = 27 })
                            .Add(this.kiteAnimations[1], new SpriteSheetAnimationSequence() { First = 10, Length = 9, FramesPerSecond = 27 })
                            .Add(this.kiteAnimations[2], new SpriteSheetAnimationSequence() { First = 19, Length = 9, FramesPerSecond = 27 }))
                        .AddComponent(new AnimatedSpriteRenderer(DefaultLayers.Alpha))
                        .AddComponent(new PerPixelCollider(Textures.KITE_COLLID, 0.5f))
                        .AddComponent(new KiteBehavior());
        }

        /// <summary>
        /// Sets a kite new color.
        /// </summary>
        public void SetNewColor()
        {
            var anim2D = this.entity.FindComponent<Animation2D>();

            anim2D.CurrentAnimation = this.kiteAnimations[WaveServices.Random.Next(0, 3)];
            anim2D.PlayToFrame(0);
        }
    }
}
