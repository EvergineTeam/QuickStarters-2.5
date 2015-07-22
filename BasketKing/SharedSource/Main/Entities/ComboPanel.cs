#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BasketKing.Behaviors;
using BasketKing.Commons;
using BasketKing.Managers;
using WaveEngine.Common.Math;
using WaveEngine.Components.Gestures;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Components.UI;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Physics2D;
using WaveEngine.Framework.Services;
#endregion

namespace BasketKing.Entities
{
    public class ComboPanel : BaseDecorator
    {
        private TextBlock comboText;

        private int multiplier;

        #region Properties
        public int Multiplier
        {
            get
            {
                return this.multiplier;
            }

            private set
            {
                this.multiplier = value;
                this.comboText.Text = string.Format("X{0}", this.multiplier);
            }
        }
        #endregion

        public ComboPanel(Vector2 position)
        {
            this.multiplier = 1;

            // Text
            this.comboText = new TextBlock()
            {
                Width = 151,
                Text = "",
                FontPath = Directories.Fonts + "Gotham Bold_16.wpk",
                TextAlignment = TextAlignment.Center,
            };
            var transform = this.comboText.Entity.FindComponent<Transform2D>();
            transform.X = -45;
            transform.Y = -17;

            // Flames
            this.entity = new Entity("ComboPanel")
                        {
                            IsVisible = false
                        }
                        .AddComponent(new Transform2D()
                        {
                            X = position.X,
                            Y = position.Y,
                            Origin = Vector2.Center
                        })
                        .AddComponent(new ShakeBehavior())
                        .AddComponent(new Sprite(Directories.Textures + "bg_combo.wpk"))
                        .AddComponent(new SpriteRenderer(DefaultLayers.Alpha))
                        .AddChild(this.comboText.Entity);
        }

        public void Clear()
        {
            this.entity.IsVisible = false;
            this.Multiplier = 1;
        }

        public void Increase()
        {
            this.entity.IsVisible = true;
            this.Multiplier++;
            this.entity.FindComponent<ShakeBehavior>().DoEffect();
            SoundsManager.Instance.PlaySound(SoundsManager.SOUNDS.Combo);
        }
    }
}
