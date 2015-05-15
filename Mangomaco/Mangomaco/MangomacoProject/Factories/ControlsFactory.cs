#region Using Statements
using MangomacoProject.Entities;
using MangomacoProject.Services;
using MangomacoProject.Scenes;
using WaveEngine.Components;
using WaveEngine.Components.UI;
using WaveEngine.Framework.Services;
using WaveEngine.Framework.UI;
using WaveEngine.Common.Graphics;
#endregion

namespace MangomacoProject.Factories
{
    /// <summary>
    /// Controls Factory
    /// </summary>
    public class ControlsFactory
    {
        /// <summary>
        /// Creates the play button.
        /// </summary>
        /// <returns>Button entity</returns>
        public static Button CreatePlayButton()
        {
            var btn = new Button()
            {
                BackgroundImage = Resources.PlayBtn,
                PressedBackgroundImage = Resources.PlayBtnPressed,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                IsBorder = false,
                Text = string.Empty
            };

            btn.Click += (s, e) =>
            {
                var soundManager = WaveServices.GetService<SimpleSoundService>();

                soundManager.PlaySound(SimpleSoundService.SoundType.Button);
                WaveServices.ScreenContextManager.Push(new ScreenContext("LevelSelection", new LevelSelectionScene()), Resources.DefaultTransition);
            };

            return btn;
        }

        /// <summary>
        /// Creates the pause button.
        /// </summary>
        /// <returns>Button entity</returns>
        public static Button CreatePauseButton()
        {
            var btn = new Button()
            {
                BackgroundImage = Resources.PauseBtn,
                PressedBackgroundImage = Resources.PauseBtnPressed,
                Margin = new Thickness(20),
                IsBorder = false,
                Text = string.Empty
            };

            btn.Click += (s, e) =>
            {
                var soundManager = WaveServices.GetService<SimpleSoundService>();

                soundManager.PlaySound(SimpleSoundService.SoundType.Button);
                WaveServices.ScreenContextManager.Push(new ScreenContext(new PauseScene()), Resources.DefaultTransition);
            };

            return btn;
        }

        /// <summary>
        /// Creates the resume button.
        /// </summary>
        /// <returns>Button entity</returns>
        public static Button CreateResumeButton()
        {
            var btn = new Button()
            {
                BackgroundImage = Resources.ResumeBtn,
                PressedBackgroundImage = Resources.ResumeBtnPressed,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                IsBorder = false,
                Text = string.Empty
            };

            btn.Click += (s, e) =>
            {
                var soundManager = WaveServices.GetService<SimpleSoundService>();

                soundManager.PlaySound(SimpleSoundService.SoundType.Button);
                WaveServices.ScreenContextManager.Pop(Resources.DefaultTransition);
            };

            return btn;
        }

        /// <summary>
        /// Creates the restart button.
        /// </summary>
        /// <returns>Button entity</returns>
        public static Button CreateRestartButton()
        {
            var btn = new Button()
            {
                BackgroundImage = Resources.RestartBtn,
                PressedBackgroundImage = Resources.RestartBtnPressed,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                IsBorder = false,
                Text = string.Empty
            };

            btn.Click += (s, e) =>
            {
                var gameplayScene = WaveServices.ScreenContextManager.FindContextByName("Gameplay")[0] as GameplayScene;
                gameplayScene.ResetGame();

                var soundManager = WaveServices.GetService<SimpleSoundService>();

                soundManager.PlaySound(SimpleSoundService.SoundType.Button);
                WaveServices.ScreenContextManager.Pop(Resources.DefaultTransition);
            };

            return btn;
        }

        /// <summary>
        /// Creates the selection button.
        /// </summary>
        /// <returns>Button entity</returns>
        public static Button CreateSelectionButton()
        {
            var btn = new Button()
            {
                BackgroundImage = Resources.SelectionBtn,
                PressedBackgroundImage = Resources.SelectionBtnPressed,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                IsBorder = false,
                Text = string.Empty
            };

            btn.Click += (s, e) =>
            {
                var soundManager = WaveServices.GetService<SimpleSoundService>();

                soundManager.PlaySound(SimpleSoundService.SoundType.Button);
                WaveServices.ScreenContextManager.Pop();
                WaveServices.ScreenContextManager.Pop(Resources.DefaultTransition);
            };

            return btn;
        }

        /// <summary>
        /// Creates the close button.
        /// </summary>
        /// <returns>Button entity</returns>
        public static Button CreateCloseButton()
        {
            var btn = new Button()
            {
                BackgroundImage = Resources.CloseBtn,
                PressedBackgroundImage = Resources.CloseBtnPressed,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Bottom,
                IsBorder = false,
                Text = string.Empty
            };

            btn.Click += (s, e) =>
            {
                var soundManager = WaveServices.GetService<SimpleSoundService>();

                soundManager.PlaySound(SimpleSoundService.SoundType.Button);
                WaveServices.ScreenContextManager.Pop(Resources.DefaultTransition);
            };

            return btn;
        }

        /// <summary>
        /// Creates the level button.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="isLocked">if set to <c>true</c> [is locked].</param>
        /// <param name="levelDefinition">The level definition.</param>
        /// <returns>
        /// Button entity
        /// </returns>
        public static Button CreateLevelButton(int index, bool isLocked, LevelDefinition levelDefinition)
        {
            var btn = new Button()
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                BackgroundImage = isLocked ? Resources.LevelBtnLocked : Resources.LevelBtn,
                PressedBackgroundImage = isLocked ? Resources.LevelBtnLocked : Resources.LevelBtnPressed,
                IsBorder = false,
                FontPath = Resources.SkranjiFont,
                Text = (index + 1).ToString(),
                Foreground = Color.White,
            };

            if (!isLocked && levelDefinition != null)
            {
                btn.Click += (s, e) =>
                {
                    ScreenContext screenContext = new ScreenContext("Gameplay", new GameplayScene(levelDefinition), new JoystickScene())
                    {
                        Behavior = ScreenContextBehaviors.DrawInBackground
                    };
                    WaveServices.ScreenContextManager.Push(screenContext, Resources.DefaultTransition);
                };
            }

            return btn;
        }

        /// <summary>
        /// Creates the mute button.
        /// </summary>
        /// <returns>ToggleButton entity</returns>
        public static ToggleButton CreateMuteButton()
        {
            var gameStorage = Catalog.GetItem<GameStorage>();

            var muteBtn = new ToggleButton()
            {
                IsBorder = false,
                IsChecked = gameStorage.IsMuted,
                CheckedImage = Resources.MusicBtnPressed,
                CheckedPressedImage = Resources.MusicBtnPressed,
                UncheckedImage = Resources.MusicBtn,
                UncheckedPressedImage = Resources.MusicBtn,
            };
            muteBtn.Checked += (s, o) =>
            {
                var soundManager = WaveServices.GetService<SimpleSoundService>();

                soundManager.PlaySound(SimpleSoundService.SoundType.Button);
                soundManager.Mute = o.Value;
                gameStorage.IsMuted = muteBtn.IsChecked;
            };

            return muteBtn;
        }

        /// <summary>
        /// Creates the pause bg panel.
        /// </summary>
        /// <returns>Image entity</returns>
        public static Image CreatePauseBgPanel()
        {
            return new Image(Resources.PauseBgPanel)
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                };
        }

        /// <summary>
        /// Creates the selection bg panel.
        /// </summary>
        /// <returns>Image entity</returns>
        public static Image CreateSelectionBgPanel()
        {
            return new Image(Resources.LevelSelectBgPanel)
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };
        }
    }
}
