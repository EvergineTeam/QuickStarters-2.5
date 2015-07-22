#region Using Statements
using BasketKing.Commons;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Graphics;
using WaveEngine.Components.UI;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.UI;
#endregion

namespace BasketKing.Entities
{
    public class ScoreboardPanel : BaseDecorator
    {
        private const int timeSeconds = 60;

        private int scores;
        private int numLaunches;
        private int numSuccesses;
        private int inARow;
        //private int multiplier;
        private TimeSpan time;
        private TextBlock scoreText, timeText;
        private Image timeIn, timeOut;
        private ComboPanel comboPanel;

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
        /// Gets or sets the time.
        /// </summary>
        public TimeSpan Time
        {
            get { return time; }
            set
            {
                time = value;
                this.timeText.Text = this.time.ToString(@"ss\:ff");

                this.timeIn.IsVisible = (time > TimeSpan.Zero) ? true : false;
                this.timeOut.IsVisible = (time > TimeSpan.Zero) ? false : true;
            }
        }

        /// <summary>
        /// Gets or sets the scores.
        /// </summary>
        public int Scores
        {
            get { return this.scores; }
            set
            {
                this.scores = value;
                this.scoreText.Text = string.Format("{0:0000}", this.scores);
            }
        }

        /// <summary>
        /// Gets or sets the num launches.
        /// </summary>
        public int NumLaunches
        {
            get { return this.numLaunches; }
            set { this.numLaunches = value; }
        }

        /// <summary>
        /// Gets or sets the number of launch success
        /// </summary>
        public int NumSuccesses
        {
            get { return this.numSuccesses; }
            set
            {
                this.numSuccesses = value;
            }
        }

        /// <summary>
        /// Gets the accuracy.
        /// </summary>
        public float Accuracy
        {
            get
            {
                if (this.numLaunches == 0)
                {
                    return 0;
                }
                else
                {
                    return ((float)this.numSuccesses / (float)this.numLaunches) * 100;
                }
            }
        }

        /// <summary>
        /// Gets or sets launch success in a row;
        /// </summary>
        public int InARow
        {
            get { return this.inARow; }
            set
            {                
                this.inARow = value;
                if (this.inARow == 0)
                {
                    //this.multiplier = 1;
                    this.comboPanel.Clear();
                }
                else if (this.inARow % 3 == 0)
                {
                    //this.multiplier++;
                    this.comboPanel.Increase();
                }
                //Debug.WriteLine("inARow:" + this.inARow + " Multiplier: " + this.Multipler);
            }
        }

        /// <summary>
        /// Gets the multiplier
        /// </summary>
        public int Multipler
        {
            get
            {
                //return this.multiplier;
                return this.comboPanel.Multiplier;
            }
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ScoreboardPanel" /> class.
        /// </summary>
        public ScoreboardPanel(ComboPanel comboPanel)
        {
            this.comboPanel = comboPanel; 

            this.entity = new Entity("scoreboardPanel")
                                .AddComponent(new Transform2D())
                                .AddComponent(new PanelControl(345, 96))
                                .AddComponent(new PanelControlRenderer())
                                .AddChild(new Image(Directories.Textures + "scoreboard.wpk")
                                {
                                    DrawOrder = 0.6f,
                                }.Entity);

            this.time = TimeSpan.FromSeconds(timeSeconds);

            // Time
            this.timeText = new TextBlock("timeText")
            {
                Width = 132,
                Height = 42,
                FontPath = Directories.Fonts + "spritefont_time.wpk",
                Text = this.time.ToString(@"ss\:ff"),
                Margin = new Thickness(34, 19, 0, 0),
                Foreground = Color.LightGreen
            };
            this.entity.AddChild(this.timeText.Entity);

            // Scores
            this.scoreText = new TextBlock("scoreText")
            {
                Width = 132,
                Height = 42,
                FontPath = Directories.Fonts + "spritefont_time.wpk",
                Text = string.Format("{0:0000}", this.scores),
                Margin = new Thickness(207, 19, 0, 0),
            };
            this.entity.AddChild(this.scoreText.Entity);

            // Lights
            this.timeIn = new Image(Directories.Textures + "scoreboard_timein.wpk")
            {
                DrawOrder = 0.45f,
                Margin = new Thickness(162, 27, 0, 0),
            };
            this.entity.AddChild(this.timeIn.Entity);

            this.timeOut = new Image(Directories.Textures + "scoreboard_timeout.wpk")
            {
                DrawOrder = 0.45f,
                Margin = new Thickness(162, 42, 0, 0),
            };
            this.entity.AddChild(this.timeOut.Entity);
        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        public void Reset()
        {
            this.Time = TimeSpan.FromSeconds(timeSeconds);
            this.Scores = 0;
            this.NumLaunches = 0;
            this.numSuccesses = 0;
            this.inARow = 0;
            this.comboPanel.Clear();
        }        
    }
}
