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
using WaveEngine.Common.Graphics;
using WaveEngine.Components.UI;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
using WaveEngine.Framework.UI;
#endregion

namespace SuperSquidProject.Entities
{
    public class ScorePanel : StackPanel
    {
        private TextBlock scoreText;

        private int score;

        public int Score
        {
            get
            {
                return this.score;
            }

            set
            {
                this.score = value;
                this.scoreText.Text = string.Format("x{0}", this.score);
            }
        }

        public ScorePanel()
        {
            this.Width = (int)WaveServices.ViewportManager.VirtualWidth;
            this.Height = 76;
            //this.BackgroundColor = Color.Black;
            this.HorizontalAlignment = HorizontalAlignment.Right;
            this.VerticalAlignment = VerticalAlignment.Bottom;
            this.Orientation = Orientation.Horizontal;
            this.Margin = new Thickness(10);
            //this.Opacity = 0.7f;


            this.Add(new Image(Directories.TexturePath + "starfish.wpk")
                                {
                                    Width = 70,
                                    Height = 70,
                                    Stretch = Stretch.Fill,
                                    DrawOrder = 0.1f,
                                    VerticalAlignment = VerticalAlignment.Center,
                                    Margin = new Thickness(5),
                                });

            this.scoreText = new TextBlock("scoresText")
            {
                Width = 80,
                FontPath = Directories.FontsPath + "Bulky Pixels_26.wpk",
                VerticalAlignment = VerticalAlignment.Center,
                TextAlignment = WaveEngine.Components.UI.TextAlignment.Center,
                Margin = new Thickness(5, 10, 5, 5),
                DrawOrder = 0.1f,
            };
            this.Add(this.scoreText);

            this.Score = 0;
        }
    }
}
