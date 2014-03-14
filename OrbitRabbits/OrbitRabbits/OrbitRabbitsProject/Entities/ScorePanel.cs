#region File Description
//-----------------------------------------------------------------------------
// OrbitRabbits
//
// Quickstarter for Wave University Tour 2014.
// Author: Wave Engine Team
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using OrbitRabbitsProject.Commons;
using OrbitRabbitsProject.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Math;
using WaveEngine.Components;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Components.UI;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
using WaveEngine.Framework.UI;
#endregion

namespace OrbitRabbitsProject.Entities
{
    public class ScorePanel : BaseDecorator
    {
        private int scores;        
        private TextBlock scoreText, bestText;
        private GameStorage gameStorage;

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
        /// Gets or sets the scores.
        /// </summary>
        public int Scores
        {
            get { return this.scores; }
            set
            {
                this.scores = value;
                this.scoreText.Text = this.scores.ToString();
                if (this.gameStorage.BestScore < this.scores)
                {
                    this.gameStorage.BestScore = this.scores;
                    this.bestText.Text = this.gameStorage.BestScore.ToString();
                }
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ScorePanel" /> class.
        /// </summary>
        public ScorePanel()
        {
            this.scores = 1;

            this.gameStorage = Catalog.GetItem<GameStorage>();

            this.entity = new Entity()
                          .AddComponent(new Transform2D()
                          {
                              Y = WaveServices.ViewportManager.TopEdge
                          })
                          .AddComponent(new PanelControl(241, 104))
                          .AddComponent(new PanelControlRenderer())
                           .AddChild(new Image(Directories.TexturePath + "scorePanel.wpk")
                           {
                               DrawOrder = 0.4f,
                           }.Entity);
            this.scoreText = new TextBlock("scoresText")
             {
                 Width = 40,
                 Text = this.Scores.ToString(),
                 FontPath = Directories.FontsPath + "OCR A Std_20.wpk",
                 HorizontalAlignment = HorizontalAlignment.Right,
                 VerticalAlignment = VerticalAlignment.Top,
                 Margin = new Thickness(0, 15, 50, 0),
                 DrawOrder = 0.2f,
             };
            this.entity.AddChild(this.scoreText.Entity);
            this.bestText = new TextBlock("bestText")
             {
                 Width = 40,
                 Text = this.gameStorage.BestScore.ToString(),
                 FontPath = Directories.FontsPath + "OCR A Std_20.wpk",
                 HorizontalAlignment = HorizontalAlignment.Right,
                 VerticalAlignment = VerticalAlignment.Bottom,
                 Margin = new Thickness(0, 0, 50, 5),
                 DrawOrder = 0.2f,
             };
            this.entity.AddChild(this.bestText.Entity);
        }
    }
}
