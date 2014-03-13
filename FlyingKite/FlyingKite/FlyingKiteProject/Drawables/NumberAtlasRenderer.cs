using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;

namespace FlyingKiteProject.Drawables
{
    public class NumberAtlasRenderer : Drawable2D
    {
        /// <summary>
        /// Transform of the <see cref="SpriteAtlas"/>.
        /// </summary>
        [RequiredComponent]
        private Transform2D transform2D;

        /// <summary>
        /// <see cref="SpriteAtlas"/> to render.
        /// </summary>
        [RequiredComponent(isExactType: false)]
        private SpriteAtlas sprite;

        public float LetterSpacing { get; set; }

        public int Number
        {
            get
            {
                return this.number;
            }

            set
            {
                this.number = value;
                this.numberToRender = value.ToString().ToCharArray();
            }
        }

        public string TextureFormat { get; set; }

        public float LeftSide { get; private set; }

        private int number;

        private char[] numberToRender;

        /// <summary>
        /// The position
        /// </summary>
        private Vector2 position;

        public NumberAtlasRenderer(Type layerType)
            : base(layerType)
        {
            this.position = Vector2.Zero;

            this.numberToRender = new char[] { };
            this.TextureFormat = "{0}";
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && this.sprite != null)
            {
                this.sprite.Dispose();
            }
        }

        protected override void DrawBasicUnit(int parameter)
        {
            if (this.transform2D.Opacity > this.Delta)
            {
                var totalWidth = this.numberToRender.Length * this.sprite.SourceRectangle.Width * this.transform2D.XScale;
                totalWidth += (this.numberToRender.Length - 1) * this.LetterSpacing;
                var totalHeight = this.sprite.SourceRectangle.Height * this.transform2D.YScale;

                this.position.X = this.transform2D.Rectangle.X + this.transform2D.X;
                this.position.Y = this.transform2D.Rectangle.Y + this.transform2D.Y;

                float opacity = this.RenderManager.DebugLines ? this.DebugAlpha : this.transform2D.Opacity;
                Color color = this.sprite.TintColor * opacity;

                this.position.X -= this.transform2D.Origin.X * totalWidth;
                this.position.Y -= this.transform2D.Origin.Y * totalHeight;

                this.LeftSide = this.position.X;

                foreach (var item in this.numberToRender)
                {
                    this.sprite.TextureName = string.Format(this.TextureFormat, item);

                    this.spriteBatch.DrawVM(
                        this.sprite.TextureAtlas.Texture,
                        this.position,
                        this.sprite.SourceRectangle,
                        color,
                        this.transform2D.Rotation,
                        Vector2.Zero,
                        Vector2.One,
                        this.transform2D.Effect,
                        this.transform2D.DrawOrder);

                    this.position.X += (this.sprite.SourceRectangle.Width + this.LetterSpacing);
                }

                this.transform2D.Rectangle.Width = totalWidth;
            }
        }
    }
}
