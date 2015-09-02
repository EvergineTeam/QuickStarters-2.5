#region Using Statements
using BasketKing.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Components.UI;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.UI;
#endregion

namespace BasketKing.Entities
{
    public class HighScorePanel : BaseDecorator
    {
        private int scores;       
        private TextBlock scoreText;

        #region Properties
        /// <summary>
        /// Gets or sets the horizontal alignment.
        /// </summary>
        public HorizontalAlignment HorizontalAlignment
        {
            get
            {
                return this.entity.FindComponent<PanelControl>().HorizontalAlignment;
            }

            set
            {
                this.entity.FindComponent<PanelControl>().HorizontalAlignment = value;
            }
        }

        /// <summary>
        /// Gets or sets the vertical alignment.
        /// </summary>
        public VerticalAlignment VerticalAlignment
        {
            get
            {
                return this.entity.FindComponent<PanelControl>().VerticalAlignment;
            }

            set
            {
                this.entity.FindComponent<PanelControl>().VerticalAlignment = value;
            }
        }

        /// <summary>
        /// Gets or sets the margin.
        /// </summary>
        /// <value>
        /// The margin.
        /// </value>
        public Thickness Margin
        {
            get
            {
                return this.entity.FindComponent<PanelControl>().Margin;
            }

            set
            {
                this.entity.FindComponent<PanelControl>().Margin = value;
            }
        }

        /// <summary>
        /// Gets or sets the scores.
        /// </summary>
        public int Scores
        {
            get { return scores; }
            set 
            {
                scores = value;
                this.scoreText.Text = string.Format("{0:0000}", this.scores);
            }
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="HighScorePanel" /> class.
        /// </summary>
        public HighScorePanel()
        {
            this.entity = new Entity("highScorePanel")
                                .AddComponent(new Transform2D())
                                .AddComponent(new PanelControl(143, 63))
                                .AddComponent(new PanelControlRenderer())
                                .AddChild(new Image(WaveContent.Assets.Textures.bg_highscore_PNG)
                                {
                                    DrawOrder = 0.6f,
                                }.Entity);

            this.scoreText = new TextBlock("scoreText")
            {
                Width = 132,
                Height = 42,
                FontPath = WaveContent.Assets.Fonts.spritefont_time_ttf,
                Text = string.Format("{0:0000}", this.scores),
                Margin = new Thickness(20, 14, 0, 0),
            };
            this.entity.AddChild(this.scoreText.Entity);                              
        }
    }
}
