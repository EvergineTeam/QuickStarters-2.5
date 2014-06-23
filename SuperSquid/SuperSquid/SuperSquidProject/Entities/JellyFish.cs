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
using WaveEngine.Framework.Animation;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Physics2D;
#endregion

namespace SuperSquidProject.Entities
{
    public class JellyFish : BaseDecorator
    {
        public enum JellyFishType
        {
            Big,
            Little,
        };

        private SingleAnimation leftAnim, rightAnim;
        private AnimationUI animation;

        public Collider2D Collider
        {
            get;
            private set;
        }

        public JellyFish(JellyFishType type, Vector2 position, bool rightAnimation = true)
        {
            this.entity = new Entity()
                               .AddComponent(new Transform2D()
                               {
                                   Origin = Vector2.Center,
                                   X = position.X,
                                   Y = position.Y,
                                   DrawOrder = 0.3f,
                               })                               
                               .AddComponent(new AnimationUI());

            // Cached
            this.animation = this.entity.FindComponent<AnimationUI>();


            string textureName, colliderName;
            switch (type)
            {
                case JellyFishType.Big:
                    textureName = "jellyFishSpriteSheet";     
                    colliderName = "jellyFishCollider.wpk";
                    break;
                case JellyFishType.Little:
                    textureName = "jellyFishLittleSpriteSheet";
                    colliderName = "jellyFishLittleCollider.wpk";
                    break;
                default:
                    textureName = "jellyFishSpriteSheet";
                    colliderName = "jellyFishCollider.wpk";
                    break;
            }

            this.entity.AddComponent(new PerPixelCollider(Directories.TexturePath + colliderName, 0.5f) { IsGlobalAsset = true});
            this.entity.AddComponent(new Sprite(Directories.TexturePath + string.Format("{0}.wpk", textureName)) { IsGlobalAsset = true});
            this.entity.AddComponent(Animation2D.Create<TexturePackerGenericXml>(Directories.TexturePath + string.Format("{0}.xml", textureName))
                                                       .Add("swim", new SpriteSheetAnimationSequence() { First = 1, Length = 40, FramesPerSecond = 30 }));
            this.entity.AddComponent(new AnimatedSpriteRenderer(DefaultLayers.Alpha));

            this.entity.FindComponent<Animation2D>().Play(true);


            // Animations
            float offset = 80;
            this.leftAnim = new SingleAnimation(position.X + offset, position.X, TimeSpan.FromSeconds(3));
            this.leftAnim.Completed += (s, o) =>
            {
                this.animation.BeginAnimation(Transform2D.XProperty, this.rightAnim);
            };
            this.rightAnim = new SingleAnimation(position.X, position.X + offset, TimeSpan.FromSeconds(3));
            this.rightAnim.Completed += (s, o) =>
            {
                this.animation.BeginAnimation(Transform2D.XProperty, this.leftAnim);
            };

            if (rightAnimation)
            {
                this.animation.BeginAnimation(Transform2D.XProperty, this.rightAnim);
            }
            else
            {
                this.animation.BeginAnimation(Transform2D.XProperty, this.leftAnim);
            }

            this.Collider = this.entity.FindComponent<Collider2D>(false);
        }
    }
}
