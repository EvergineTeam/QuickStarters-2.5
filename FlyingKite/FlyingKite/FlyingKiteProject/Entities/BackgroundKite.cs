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
using WaveEngine.Components.Graphics2D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Physics2D;
using WaveEngine.Framework.Services;
#endregion

namespace FlyingKiteProject.Entities
{
    public class BackgroundKite : BaseDecorator
    {
        public BackgroundKite(float initialX)
        {
            var scale = this.GetRandomScale();

            var scrollBehavior = new ScrollBehavior(scale * 0.1f);

            var transform = new Transform2D()
            {
                X = initialX,
                Y = WaveServices.ViewportManager.VirtualHeight,
                Origin = Vector2.UnitY,
                XScale = scale,
                YScale = scale
            };

            var spriteAtlas = new SpriteAtlas(Textures.GAME_ATLAS, this.GetRandomTextureName());

            this.entity = new Entity()
                .AddComponent(transform)
                .AddComponent(spriteAtlas)
                .AddComponent(new SpriteAtlasRenderer(DefaultLayers.Alpha))
                .AddComponent(scrollBehavior);

            scrollBehavior.EntityOutOfScreen += (entity) =>
            {
                transform.X = WaveServices.ViewportManager.RightEdge;

                var newScale = this.GetRandomScale();
                transform.XScale = newScale;
                transform.YScale = newScale;

                //Set a new kite texture
                spriteAtlas.TextureName = this.GetRandomTextureName();
            };
        }

        private float GetRandomScale()
        {
            return (WaveServices.Random.Next(2) * 0.1f) + 0.8f;
        }

        private string GetRandomTextureName()
        {
            return (WaveServices.Random.NextBool() ? Textures.GameAtlas.bg_fish_kite_01 : Textures.GameAtlas.bg_fish_kite_02).ToString();
        }
    }
}
