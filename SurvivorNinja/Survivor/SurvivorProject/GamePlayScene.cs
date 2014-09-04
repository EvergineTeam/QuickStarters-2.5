#region Using Statements
using System;
using System.Linq;
using SurvivorProject.Commons;
using SurvivorProject.Entities;
using WaveEngine.Common;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
using WaveEngine.Common.Media;
using WaveEngine.Components.UI;
using WaveEngine.Framework.UI;
using WaveEngine.Components;
using SurvivorProject.Managers;
using WaveEngine.Components.Cameras;
#endregion

namespace SurvivorProject
{
    public class GamePlayScene : Scene
    {
        public enum States
        {
            Menu,
            GamePlay,
            GameOver,
        };

        private Player player;
        private MusicInfo musicInfo;
        private States currentState;

        private HubPanel hubPanel;
        private Image logo, tap_to_start, game_over;
        private TextBlock bestScores;
        private BulletEmiter bulletEmiter;
        private EnemyEmiter enemyEmiter;
        private Joystick leftJoystick, rightJoystick;

        #region Properties
        /// <summary>
        /// Gets or sets the state of the current.
        /// </summary>
        public States CurrentState
        {
            get { return currentState; }
            set
            {
                this.UpdateState(value);
            }
        }
        #endregion

        protected override void CreateScene()
        {
            FixedCamera2D camera2d = new FixedCamera2D("camera");
            camera2d.BackgroundColor = Color.CornflowerBlue;
            EntityManager.Add(camera2d);

            // Background
            Entity background = new Entity()
                                    .AddComponent(new Transform2D()
                                    {
                                        X = WaveServices.ViewportManager.LeftEdge,
                                        Y = WaveServices.ViewportManager.TopEdge,
                                        DrawOrder = 1f,
                                        XScale = (WaveServices.ViewportManager.ScreenWidth / 1024) / WaveServices.ViewportManager.RatioX,
                                        YScale = (WaveServices.ViewportManager.ScreenHeight / 768) / WaveServices.ViewportManager.RatioY,
                                    })
                                    .AddComponent(new Sprite(Directories.Textures + "background.wpk"))
                                    .AddComponent(new SpriteRenderer(DefaultLayers.Opaque));
            EntityManager.Add(background);

            // Player
            this.player = new Player("player", new Vector2(WaveServices.ViewportManager.VirtualWidth / 2,
                                                   WaveServices.ViewportManager.VirtualHeight / 2));
            EntityManager.Add(this.player);

            // Bullets pool
            this.bulletEmiter = new BulletEmiter("bulletEmiter");
            EntityManager.Add(this.bulletEmiter);

            // Enemy pool
            this.enemyEmiter = new EnemyEmiter("EnemyEmiter");
            EntityManager.Add(this.enemyEmiter);

            // Left Joystick
            RectangleF leftArea = new RectangleF(0,
                                                  0,
                                                  WaveServices.ViewportManager.VirtualWidth / 2f,
                                                  WaveServices.ViewportManager.VirtualHeight);
            this.leftJoystick = new Joystick("leftJoystick", leftArea);
            EntityManager.Add(this.leftJoystick);

            // Right Joystick
            RectangleF rightArea = new RectangleF(WaveServices.ViewportManager.VirtualWidth / 2,
                                                  0,
                                                  WaveServices.ViewportManager.VirtualWidth / 2f,
                                                  WaveServices.ViewportManager.VirtualHeight);
            this.rightJoystick = new Joystick("rightJoystick", rightArea);
            EntityManager.Add(this.rightJoystick);

            // Create Menu
            this.CreateMenuUI();

            // Create GameOver
            this.CreateGameOver();

            // CreateHUBUI
            this.hubPanel = new HubPanel();
            EntityManager.Add(this.hubPanel);

            // Scene behavior
            this.AddSceneBehavior(new DebugSceneBehavior(), SceneBehavior.Order.PostUpdate);
            this.AddSceneBehavior(new GamePlaySceneBehavior(), SceneBehavior.Order.PostUpdate);

            // Music
            this.musicInfo = new MusicInfo(Directories.Sounds + "ninjaMusic.mp3");
            WaveServices.MusicPlayer.Play(this.musicInfo);
            WaveServices.MusicPlayer.Volume = 0.8f;
            WaveServices.MusicPlayer.IsRepeat = true;

            this.CurrentState = States.Menu;
        }

        /// <summary>
        /// Creates the menu UI.
        /// </summary>
        private void CreateMenuUI()
        {
            // Logo
            this.logo = new Image(Directories.Textures + "logo.wpk")
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 50, 0, 0),
            };
            EntityManager.Add(this.logo);

            // Tap to start
            this.tap_to_start = new Image(Directories.Textures + "tap_to_start.wpk")
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 600, 0, 0),
            };
            EntityManager.Add(this.tap_to_start);

            // BestScores
            this.bestScores = new TextBlock()
            {
                Width = 300,
                FontPath = Directories.Fonts + "Coalition.wpk",
                Text = string.Format("BestScores: {0}", 0),
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Bottom,
                TextAlignment = TextAlignment.Right,
                Margin = new Thickness(10),
            };
            EntityManager.Add(this.bestScores);
        }

        /// <summary>
        /// Creates the game over.
        /// </summary>
        private void CreateGameOver()
        {
            // GameOver
            this.game_over = new Image(Directories.Textures + "gameover.wpk")
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };
            EntityManager.Add(this.game_over);
        }

        /// <summary>
        /// Resets the match.
        /// </summary>
        private void ResetMatch()
        {
            GameStorage gameStorage = Catalog.GetItem<GameStorage>();
            if (this.hubPanel.Murders > gameStorage.BestScore)
            {
                gameStorage.BestScore = this.hubPanel.Murders;
                WaveServices.Storage.Write<GameStorage>(gameStorage);
            }

            this.bestScores.Text = string.Format("BestScores: {0}", gameStorage.BestScore);            

            this.player.Reset();
            this.enemyEmiter.Reset();
            this.hubPanel.Reset();            
        }

        /// <summary>
        /// Updates the state.
        /// </summary>
        /// <param name="newState">The new state.</param>
        private void UpdateState(States newState)
        {
            this.logo.IsVisible = false;
            this.tap_to_start.IsVisible = false;
            this.bestScores.IsVisible = false;
            this.game_over.IsVisible = false;
            this.hubPanel.IsVisible = false;            
            this.enemyEmiter.IsActive = false;            

            this.leftJoystick.IsActive = false;            
            this.rightJoystick.IsActive = false;            

            switch (newState)
            {
                case States.Menu:
                    this.ResetMatch();
                    this.logo.IsVisible = true;
                    this.tap_to_start.IsVisible = true;
                    this.bestScores.IsVisible = true;
                    WaveServices.MusicPlayer.Volume = 0.8f;
                    break;
                case States.GamePlay:
                    SoundsManager.Instance.PlaySound(SoundsManager.SOUNDS.Ready);
                    this.hubPanel.IsVisible = true;
                    this.enemyEmiter.IsActive = true;                    
                    WaveServices.TimerFactory.CreateTimer("gamePlayTimer", TimeSpan.FromSeconds(0.5f), () =>
                    {                        
                        this.rightJoystick.IsActive = true;
                        this.leftJoystick.IsActive = true;
                        this.player.IsActive = true;
                    }, false);
                    WaveServices.MusicPlayer.Volume = 0.9f;
                    break;
                case States.GameOver:
                    SoundsManager.Instance.PlaySound(SoundsManager.SOUNDS.GameOver);
                    this.game_over.IsVisible = true;
                    this.player.IsActive = false;

                    WaveServices.TimerFactory.CreateTimer("gameOverTimer", TimeSpan.FromSeconds(2f), () =>
                    {
                        this.CurrentState = States.Menu;
                    }, false);
                    break;
                default:
                    break;
            }

            this.currentState = newState;
        }

    }
}
