using WaveEngine.Common.Graphics;
using WaveEngine.Components.UI;
using WaveEngine.Framework;

namespace MultiplayerTopDownTank.Entities
{
    public class Hub : BaseDecorator
    {
        private int playerlife;
        private ProgressBar progressBar;

        #region Properties

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

        public Hub(string name)
        {
            Grid grid = new Grid("Hub")
            {
                HorizontalAlignment = WaveEngine.Framework.UI.HorizontalAlignment.Left,
                VerticalAlignment = WaveEngine.Framework.UI.VerticalAlignment.Top,
                Width = 300,
                Height = 40
            };

            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Proportional) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Proportional) });

            Image background = new Image(WaveContent.Assets.Gui.Hub.hubBackground_png);
            background.SetValue(GridControl.RowProperty, 0);
            background.SetValue(GridControl.ColumnProperty, 0);
            grid.Add(background);

            // Life
            this.progressBar = new ProgressBar()
            {
                Height = 25,
                Width = 300,
                Margin = new WaveEngine.Framework.UI.Thickness(10,0,0,0),
                Foreground = Color.White,
                Background = Color.Red,
                HorizontalAlignment = WaveEngine.Framework.UI.HorizontalAlignment.Left,
                VerticalAlignment = WaveEngine.Framework.UI.VerticalAlignment.Center,
                DrawOrder = -1
            };

            progressBar.SetValue(GridControl.RowProperty, 0);
            progressBar.SetValue(GridControl.ColumnProperty, 0);
            grid.Add(progressBar);

            this.entity = grid.Entity;
            this.entity.Name = name;
        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        public void Reset()
        {
            this.Playerlife = 100;
        }
    }
}