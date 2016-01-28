#region Using Statements
using BasketKing.Commons;
using BasketKing.Entities;
using BasketKing.Managers;
using System;
using WaveEngine.Common;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Common.Media;
using WaveEngine.Components;
using WaveEngine.Components.Cameras;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Components.Graphics3D;
using WaveEngine.Components.UI;
using WaveEngine.Framework;
using WaveEngine.Framework.Animation;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Resources;
using WaveEngine.Framework.Services;
using WaveEngine.Framework.UI;
#endregion

namespace BasketKing
{
    public class GamePlayScene : Scene
    {
        public enum States
        {
            TapToStart,
            HurryUp,
            Ready,
            GamePlay,
            TimeOut,
            Stadistics,
        };

        private States currentState;
        private HighScorePanel highScorePanel;
        public ScoreboardPanel scoreboardPanel;
        private MessagePanel messagePanel;
        private StadisticsPanel stadisticsPanel;
        private ComboPanel comboPanel;
        private Button b_pause;
        private Entity tapToStart, startBall;

        private MusicInfo bg_music, bg_ambient;

        #region Properties

        /// <summary>
        /// Gets or sets the state of the current.
        /// </summary>
        public States CurrentState
        {
            get { return this.currentState; }
            set
            {
                this.currentState = value;
                this.UpdateState(this.currentState);
            }
        }

        #endregion

        protected override void CreateScene()
        {
            this.Load(@"Content/Scenes/GamePlayScene.wscene");

            this.startBall = this.EntityManager.Find("BallStart");
            this.tapToStart = this.EntityManager.Find("TapToStart"); 

            // UI
            this.CreateUI();

            // Scene Behaviors 
            this.AddSceneBehavior(new DebugSceneBehavior(), SceneBehavior.Order.PostUpdate);
            this.AddSceneBehavior(new GamePlaySceneBehavior(), SceneBehavior.Order.PostUpdate);

            // Play background music
            this.bg_music = new MusicInfo(WaveContent.Assets.Sounds.bg_music_mp3);
            this.bg_ambient = new MusicInfo(WaveContent.Assets.Sounds.bg_ambient_mp3); 

            WaveServices.MusicPlayer.Play(this.bg_music);
            WaveServices.MusicPlayer.IsRepeat = true;

            // Set first state
            this.CurrentState = States.TapToStart;
        }       

        private void CreateUI()
        {
            // Pause button
            this.b_pause = new Button()
            {
                IsBorder = false,
                Text = string.Empty,
                BackgroundImage = WaveContent.Assets.Textures.pause_bt_PNG,
                PressedBackgroundImage = WaveContent.Assets.Textures.pause_bt_pressed_PNG,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(0, 10, 10, 0),
            };
            this.b_pause.Click += (s, o) =>
            {
                SoundsManager.Instance.PlaySound(SoundsManager.SOUNDS.Button);
                WaveServices.ScreenContextManager.Push(new ScreenContext(new PauseScene(this)));
            };
            EntityManager.Add(this.b_pause);

            // Ready! Go! Timeout!
            this.messagePanel = new MessagePanel(MessagePanel.MessageType.HurryUp)
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };
            EntityManager.Add(this.messagePanel);

            // Stadistics
            this.stadisticsPanel = new StadisticsPanel()
            {
                HorizontalAlignment = HorizontalAlignment.Center,
            };
            EntityManager.Add(this.stadisticsPanel);

            // HighScore
            this.highScorePanel = new HighScorePanel()
            {
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 100, 0, 0),
            };
            EntityManager.Add(this.highScorePanel);

            // Combo
            this.comboPanel = new ComboPanel(new Vector2(this.VirtualScreenManager.VirtualWidth / 2, this.VirtualScreenManager.TopEdge + 150));
            EntityManager.Add(this.comboPanel);

            // Scoreboard
            this.scoreboardPanel = new ScoreboardPanel(this.comboPanel)
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top,
            };
            EntityManager.Add(this.scoreboardPanel);
        }

        /// <summary>
        /// Updates the state.
        /// </summary>
        /// <param name="state">The state.</param>
        private void UpdateState(States state)
        {
            this.tapToStart.IsVisible = false;
            this.tapToStart.IsVisible = false; 
            this.b_pause.IsVisible = false;
            this.scoreboardPanel.IsVisible = false;
            this.messagePanel.IsVisible = false;
            this.stadisticsPanel.IsVisible = false;
            this.highScorePanel.IsVisible = false;
            this.comboPanel.IsVisible = false; 
            this.startBall.IsVisible = false; 

            switch (state)
            {
                case States.TapToStart:
                    this.tapToStart.IsVisible = true;
                    this.tapToStart.IsVisible = true;

                    GameStorage gameStorage = Catalog.GetItem<GameStorage>();
                    this.highScorePanel.Scores = gameStorage.BestScore;
                    this.highScorePanel.IsVisible = true;

                    WaveServices.MusicPlayer.Play(this.bg_music);
                    WaveServices.MusicPlayer.Volume = 1f;
                    break;
                case States.HurryUp:
                    this.messagePanel.Type = MessagePanel.MessageType.HurryUp;
                    SoundsManager.Instance.PlaySound(SoundsManager.SOUNDS.HurryUp);

                    WaveServices.TimerFactory.CreateTimer("timer", TimeSpan.FromSeconds(1f), () =>
                    {
                        this.CurrentState = States.Ready;
                    }, false);

                    break;
                case States.Ready:
                    this.messagePanel.Type = MessagePanel.MessageType.Ready;
                    SoundsManager.Instance.PlaySound(SoundsManager.SOUNDS.Ready);

                    WaveServices.TimerFactory.CreateTimer("timer", TimeSpan.FromSeconds(1f), () =>
                    {
                        this.CurrentState = States.GamePlay;
                    }, false);

                    break;
                case States.GamePlay:
                    SoundsManager.Instance.PlaySound(SoundsManager.SOUNDS.Go);
                    this.scoreboardPanel.IsVisible = true;
                    this.b_pause.IsVisible = true;
                    this.comboPanel.IsVisible = true;
                    this.startBall.IsVisible = true;
                    WaveServices.MusicPlayer.Play(this.bg_ambient);
                    break;

                case States.TimeOut:
                    SoundsManager.Instance.PlaySound(SoundsManager.SOUNDS.TimeOver);

                    this.messagePanel.Type = MessagePanel.MessageType.Timeout;
                    this.scoreboardPanel.IsVisible = true;
                    this.b_pause.IsVisible = true;

                    WaveServices.TimerFactory.CreateTimer("timer", TimeSpan.FromSeconds(2.5f), () =>
                    {
                        this.CurrentState = States.Stadistics;
                    }, false);

                    break;
                case States.Stadistics:
                    this.stadisticsPanel.Scores = this.scoreboardPanel.Scores;
                    this.stadisticsPanel.Accuracy = (int)this.scoreboardPanel.Accuracy;
                    this.stadisticsPanel.IsVisible = true;
                    SoundsManager.Instance.PlaySound(SoundsManager.SOUNDS.Record);
                    WaveServices.MusicPlayer.Play(this.bg_music);
                    break;
                default:
                    break;
            }
        }     
    }
}
