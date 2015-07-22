#region Using Statements
using BasketKing.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Components.UI;
using WaveEngine.Framework;
#endregion

namespace BasketKing.Entities
{
    public class ButtonOption : BaseDecorator
    {
        private Grid grid;

        #region Properties
        /// <summary>
        /// Gets the grid.
        /// </summary>
        public Grid Grid
        {
            get { return grid; }
        } 
        #endregion

        public ButtonOption(string option, Action action)
        {
            this.grid = new Grid();

            grid.HorizontalAlignment = WaveEngine.Framework.UI.HorizontalAlignment.Center;
            grid.VerticalAlignment = WaveEngine.Framework.UI.VerticalAlignment.Center;
            grid.Width = 475;
            grid.Height = 139;

            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Proportional) });

            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Proportional) });

            Button b_option = new Button()
            {
                Text = string.Empty,
                IsBorder = false,
                BackgroundImage = Directories.Textures + option + "_bt.wpk",
                PressedBackgroundImage = Directories.Textures + option + "_bt_pressed.wpk",
                VerticalAlignment = WaveEngine.Framework.UI.VerticalAlignment.Center,
                HorizontalAlignment = WaveEngine.Framework.UI.HorizontalAlignment.Center,
            };
            b_option.Click += (s, o) => action();
            b_option.SetValue(GridControl.RowProperty, 0);
            b_option.SetValue(GridControl.ColumnProperty, 0);
            grid.Add(b_option);
           
            TextBlock t_option = new TextBlock()
            {
                Width = 322, 
                Height = 65,
                FontPath = Directories.Fonts + "Gotham Bold_16.wpk",
                Text = option.ToUpper(),
                VerticalAlignment = WaveEngine.Framework.UI.VerticalAlignment.Center,
                HorizontalAlignment = WaveEngine.Framework.UI.HorizontalAlignment.Center,
                DrawOrder = 0.4f,
                Margin = new WaveEngine.Framework.UI.Thickness(0, 13, 0,0)
            };
            t_option.SetValue(GridControl.RowProperty, 0);
            t_option.SetValue(GridControl.ColumnProperty, 1);
            grid.Add(t_option);

            Image bg_option = new Image(Directories.Textures + "bg_options_pause.wpk")
            {
                VerticalAlignment = WaveEngine.Framework.UI.VerticalAlignment.Center,
                HorizontalAlignment = WaveEngine.Framework.UI.HorizontalAlignment.Center,
                Margin = new WaveEngine.Framework.UI.Thickness(-45,0,0,0),
                DrawOrder = 0.9f,
            };
            bg_option.SetValue(GridControl.RowProperty, 0);
            bg_option.SetValue(GridControl.ColumnProperty, 1);
            grid.Add(bg_option);

            this.entity = grid.Entity;
        }
    }
}
