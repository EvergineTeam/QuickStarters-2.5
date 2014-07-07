#region Using Statements
using SurvivorProject.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Graphics;
using WaveEngine.Components.UI;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
#endregion

namespace SurvivorProject.Entities
{
    public class HubPanel : BaseDecorator
    {
        private int murders;
        private int playerlife;
        private ProgressBar progressBar;
        private TextBlock murdersText;

        #region Properties

        /// <summary>
        /// Gets or sets the murders.
        /// </summary>
        public int Murders
        {
            get { return murders; }
            set 
            {
                murders = value;
                this.murdersText.Text = string.Format("#{0:00}", value);
            }
        }

        /// <summary>
        /// Gets or sets the playerlife.
        /// </summary>
        public int Playerlife
        {
            get { return playerlife; }
            set 
            {
                playerlife = value;
                if (value > 0)
                {
                    this.progressBar.Value = value;
                }
            }
        } 

        #endregion

        public HubPanel()
        {
            Grid grid = new Grid("HubPanel")
            {
                HorizontalAlignment = WaveEngine.Framework.UI.HorizontalAlignment.Center,
                VerticalAlignment = WaveEngine.Framework.UI.VerticalAlignment.Top,
                Width = 800,
                Height = 40,
            };

            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Proportional) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Proportional) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Proportional) });

            Image background = new Image(Directories.Textures + "hubBackground.wpk");
            background.SetValue(GridControl.RowProperty, 0);
            background.SetValue(GridControl.ColumnProperty, 0);
            grid.Add(background);

            // Life
            this.progressBar = new ProgressBar()
            {
                Height = 25,
                Width = 300,
                Foreground = Color.Gold,
                Background = Color.Red,
                HorizontalAlignment = WaveEngine.Framework.UI.HorizontalAlignment.Center,
                VerticalAlignment = WaveEngine.Framework.UI.VerticalAlignment.Center,
            };
            progressBar.SetValue(GridControl.RowProperty, 0);
            progressBar.SetValue(GridControl.ColumnProperty, 0);
            grid.Add(progressBar);

            // Murders
            this.murdersText = new TextBlock()
            {
                FontPath = Directories.Fonts + "Coalition.wpk",                
                Text = "#00",
                HorizontalAlignment = WaveEngine.Framework.UI.HorizontalAlignment.Center,
                VerticalAlignment = WaveEngine.Framework.UI.VerticalAlignment.Bottom,
            };
            murdersText.SetValue(GridControl.RowProperty, 0);
            murdersText.SetValue(GridControl.ColumnProperty, 1);
            grid.Add(murdersText);

            this.entity = grid.Entity;

        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        public void Reset()
        {
            this.Murders = 0;
            this.Playerlife = 100;
        }
    }
}
