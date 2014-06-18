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
using WaveEngine.Components.Animation;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Physics2D;
#endregion

namespace SuperSquidProject.Entities
{
    public class Rock : BaseDecorator
    {
        public enum RockType
        {
            Center,
            Left,
            Right,
        };

        public Collider2D Collider
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Rock" /> class.
        /// </summary>
        /// <param name="rockType">Type of the rock.</param>
        public Rock(RockType rockType, Vector2 position)
        {
            this.entity = new Entity()
                                .AddComponent(new Transform2D()
                                {
                                    X = position.X,
                                    Y = position.Y,
                                    DrawOrder = 0.2f,
                                })
                                .AddComponent(new SpriteRenderer(DefaultLayers.Alpha));

            string textureName;
            Vector2 rockOrigin, seaweedsPosition;
            switch (rockType)
            {
                case RockType.Center:
                    textureName = "centralRock";
                    rockOrigin = Vector2.Center;
                    seaweedsPosition = Vector2.Zero;
                    break;
                case RockType.Left:
                    textureName = "leftRock";
                    rockOrigin = Vector2.Zero;
                    seaweedsPosition = new Vector2(225,135);
                    break;
                case RockType.Right:
                    textureName = "rightRock";
                    rockOrigin = Vector2.UnitX;
                    seaweedsPosition = new Vector2(-170, 100);
                    break;
                default:
                    textureName = "centralRock";
                    rockOrigin = Vector2.Zero;
                    seaweedsPosition = Vector2.Zero;
                    break;
            }
            this.entity.AddComponent(new Sprite(Directories.TexturePath + string.Format("{0}.wpk", textureName)) { IsGlobalAsset = true});
            this.entity.AddComponent(new PerPixelCollider(Directories.TexturePath + string.Format("{0}Collider.wpk", textureName), 0.5f) { IsGlobalAsset = true});           
            this.entity.FindComponent<Transform2D>().Origin = rockOrigin;

            // Seaweeds
            Entity seaweeds = new Entity()
                                .AddComponent(new Transform2D()
                                {
                                    Origin = new Vector2(0.5f, 1),
                                    X = seaweedsPosition.X,
                                    Y = seaweedsPosition.Y,
                                    DrawOrder = 0.1f,
                                })
                                .AddComponent(new Sprite(Directories.TexturePath + "seaweedsSpriteSheet.wpk") { IsGlobalAsset = true})
                                .AddComponent(Animation2D.Create<TexturePackerGenericXml>(Directories.TexturePath + "seaweedsSpriteSheet.xml")
                                                        .Add("wave", new SpriteSheetAnimationSequence() { First = 1, Length = 80, FramesPerSecond = 20 }))
                                .AddComponent(new AnimatedSpriteRenderer(DefaultLayers.Alpha));
            this.entity.AddChild(seaweeds);

            seaweeds.FindComponent<Animation2D>().Play(true);

            this.Collider = this.entity.FindComponent<Collider2D>(false);
        }
    }
}
