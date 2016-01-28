#region File Description
//-----------------------------------------------------------------------------
// Flying Kite
//
// Quickstarter for Wave University Tour 2014.
// Author: Wave Engine Team
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using FlyingKite.Behaviors;
using FlyingKite.Components;
using FlyingKite.Drawables;
using FlyingKite.Layers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WaveEngine.Common.Math;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Components.Graphics3D;
using WaveEngine.Components.UI;
using WaveEngine.Framework;
using WaveEngine.Framework.Animation;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Physics2D;
using WaveEngine.Framework.Resources;
using WaveEngine.Framework.Services;
using WaveEngine.Framework.UI;
using WaveEngine.Materials;
#endregion

namespace FlyingKite
{
    public static class EntitiesFactory
    {  
        /// <summary>
        /// Creates a new BestScore panel.
        /// </summary>
        /// <param name="y">The y of the new element.</param>
        /// <param name="score">The score of the new element.</param>
        /// <returns></returns>
        public static StackPanel CreateBestScore(int y, int score)
        {
            var stackPanelScore = new StackPanel()
            {
                HorizontalAlignment = WaveEngine.Framework.UI.HorizontalAlignment.Center,
                Orientation = WaveEngine.Components.UI.Orientation.Horizontal,
                Margin = new Thickness(
                            0,
                            y,
                            0,
                            0)
            };

            var bestScoreText = new Image(WaveContent.Assets.Menus.best_score_png); 

            var bestScoreNumber = new TextBlock()
            {
                FontPath =  WaveContent.Assets.Fonts.BadaBoom_BB_wpk,
                Text = score.ToString(),
                TextAlignment = WaveEngine.Components.UI.TextAlignment.Right,
                Width = 50,
            };

            stackPanelScore.Add(bestScoreText);
            stackPanelScore.Add(bestScoreNumber);

            return stackPanelScore;
        }

        /// <summary>
        /// Creates a new CurrentScore TextBlock.
        /// </summary>
        /// <param name="y">The y of the new element.</param>
        /// <returns></returns>
        public static TextBlock CreateCurrentScore(int y, int width)
        {

            var scoreNumber = new TextBlock("currentScoreTB")
            {
                FontPath = WaveContent.Assets.Fonts.Showcard_Gothic_wpk,
                HorizontalAlignment = WaveEngine.Framework.UI.HorizontalAlignment.Center,
                TextAlignment = WaveEngine.Components.UI.TextAlignment.Center,
                Margin = new Thickness(0, y, 0, 0),
                Width = width,
                Height = 117
            };

            return scoreNumber;
        }

        /// <summary>
        /// Creates a new play button.
        /// </summary>
        /// <param name="x">The x of the new button.</param>
        /// <param name="y">The y of the new button.</param>
        /// <returns></returns>
        public static Button CreatePlayButton(float x, float y)
        {
            var button = new Button()
            {
                Margin = new Thickness(0, y, 0, 0),
                HorizontalAlignment = WaveEngine.Framework.UI.HorizontalAlignment.Center,
                Text = string.Empty,
                IsBorder = false,
                BackgroundImage = WaveContent.Assets.Menus.play_button_png,
                PressedBackgroundImage = WaveContent.Assets.Menus.play_button_pressed_png

            };

            return button;
        }
    }
}
