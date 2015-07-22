#region Using Statements
using BasketKing.Commons;
using BasketKing.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Math;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Components.UI;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
using WaveEngine.Framework.UI;
using WaveEngine.Components;
#endregion

namespace BasketKing.Entities
{
    public class StadisticsPanel : BaseDecorator
    {
        private int scores;
        private int accuracy;
        private TextBlock scoresText, accuracyText;

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
                this.scores = value;
                this.scoresText.Text = string.Format("<rtf Foreground=\"#FECC56\">SCORES: </rtf>{0}", this.scores);

                GameStorage gameStorage = Catalog.GetItem<GameStorage>();
                if (this.scores > gameStorage.BestScore)
                {
                    gameStorage.BestScore = this.scores;
                }
            }
        }

        /// <summary>
        /// Gets or sets the accuracy.
        /// </summary>
        public int Accuracy
        {
            get { return accuracy; }
            set
            {
                this.accuracy = value;                

                this.accuracyText.Text = string.Format("<rtf Foreground=\"#FECC56\">ACCURACY: </rtf>{0}%", this.accuracy);
            }
        }        

        #endregion

        public StadisticsPanel()
        {
            this.scores = 0;
            this.accuracy = 0;

            this.entity = new Entity("StadisticsPanel")
                                .AddComponent(new Transform2D())
                                .AddComponent(new PanelControl(1024, 460))
                                .AddComponent(new PanelControlRenderer())
                                .AddChild(new Image(Directories.Textures + "bg_final_score.wpk")
                                {
                                    DrawOrder = 0.6f,
                                }.Entity);

            // Score text
            this.scoresText = new TextBlock()
            {
                FontPath = Directories.Fonts + "Gotham Bold_16.wpk",
                Width = 450,
                Height = 80,
                Text = "<rtf Foreground=\"#FECC56\">SCORES: </rtf>" + scores,
                RichTextEnabled = true,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 200, 0, 0),
            };
            this.entity.AddChild(this.scoresText.Entity);

            // Accuracy text
            this.accuracyText = new TextBlock()
            {
                FontPath = Directories.Fonts + "Gotham Bold_16.wpk",
                Width = 450,
                Height = 80,
                RichTextEnabled = true,
                Text = "<rtf Foreground=\"#FECC56\">ACCURACY: </rtf>" + accuracy + "%",
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 280, 0, 0),
            };
            this.entity.AddChild(this.accuracyText.Entity);

            // Restart button
            Button restart = new Button()
            {
                Text = string.Empty,
                IsBorder = false,
                BackgroundImage = Directories.Textures + "restart_bt.wpk",
                PressedBackgroundImage = Directories.Textures + "restart_bt_pressed.wpk",
                Margin = new Thickness(351, 382, 0, 0),
            };
            restart.Click += (s, o) =>
            {
                var currentContext = WaveServices.ScreenContextManager.CurrentContext;
                GamePlayScene gamePlayScene = currentContext.FindScene<GamePlayScene>();
                gamePlayScene.CurrentState = GamePlayScene.States.HurryUp;
                SoundsManager.Instance.PlaySound(SoundsManager.SOUNDS.Button);
            };
            this.entity.AddChild(restart.Entity);

            // Exit button
            Button exit = new Button()
            {
                Text = string.Empty,
                IsBorder = false,
                BackgroundImage = Directories.Textures + "exit_bt.wpk",
                PressedBackgroundImage = Directories.Textures + "exit_bt_pressed.wpk",
                Margin = new Thickness(552, 382, 0, 0),
            };
            exit.Click += (s, o) =>
            {
                var currentContext = WaveServices.ScreenContextManager.CurrentContext;
                GamePlayScene gamePlayScene = currentContext.FindScene<GamePlayScene>();
                gamePlayScene.CurrentState = GamePlayScene.States.TapToStart;
                SoundsManager.Instance.PlaySound(SoundsManager.SOUNDS.Button);
            };
            this.entity.AddChild(exit.Entity);
        }      
    }
}
