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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Math;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Physics2D;
using WaveEngine.Framework.Services; 
#endregion

namespace SuperSquidProject.Entities
{
    public class StarFish : BaseDecorator
    {
        public Collider2D collider;

        public Transform2D Transform2D;

        public StarFish(Vector2 position)
        {
            float sizeRandom = WaveServices.Random.Next(0, 5) /10f;

            this.entity = new Entity()
                            .AddComponent(new Transform2D()
                            {
                                Origin = Vector2.Center,
                                X = position.X,
                                Y = position.Y,
                                XScale = 0.6f + sizeRandom,
                                YScale = 0.6f + sizeRandom,
                            })
                            .AddComponent(new CircleCollider())
                            .AddComponent(new Sprite(Directories.TexturePath + "starfish.wpk"))
                            .AddComponent(new SpriteRenderer(DefaultLayers.Alpha));

            // Cached
            this.collider = this.entity.FindComponent<CircleCollider>();
            this.Transform2D = this.entity.FindComponent<Transform2D>();
        }
    }
}
