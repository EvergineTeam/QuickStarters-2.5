#region Using Statements
using System;
using WaveEngine.Common.Graphics;
using WaveEngine.Components.Transitions;
#endregion

namespace MangomacoProject
{
    /// <summary>
    /// Resource class
    /// </summary>
    public class Resources
    {
        /// <summary>
        /// The gameplay gb color
        /// </summary>
        public static readonly Color GameplayGBColor = new Color("#5a93e0");

        /// <summary>
        /// Gets the default transition.
        /// </summary>
        public static CrossFadeTransition DefaultTransition
        {
            get
            {
                return new CrossFadeTransition(TimeSpan.FromMilliseconds(200));
            }
        }

        // Miscellaneous
        public static readonly string CoinTP = "Content/coin.xml";

        // Fonts
        public static readonly string SkranjiFont = "Content/Fonts/Skranji-Regular.spr";

        // Images
        public static readonly string MenuBackground = "Content/menuBackground.jgp";
        public static readonly string Ball = "Content/ball.png";
        public static readonly string Coin = "Content/coin.png";
        public static readonly string SceneTiles = "Content/tilesets/sceneTiles.png";

        public static readonly string PlayBtn = "Content/Menu/play_btn.png";
        public static readonly string PlayBtnPressed = "Content/Menu/play_btn_pressed.png";
        public static readonly string PauseBtn = "Content/Menu/pause_btn.png";
        public static readonly string PauseBtnPressed = "Content/Menu/pause_btn_pressed.png";
        public static readonly string MusicBtn = "Content/Menu/music_btn.png";
        public static readonly string MusicBtnPressed = "Content/Menu/music_btn_pressed.png";
        public static readonly string SoundBtn = "Content/Menu/sound_btn.png";
        public static readonly string SoundBtnPressed = "Content/Menu/sound_btn_pressed.png";

        public static readonly string PauseBgPanel = "Content/Menu/pause_bg.png";
        public static readonly string ResumeBtn = "Content/Menu/resume_btn.png";
        public static readonly string ResumeBtnPressed = "Content/Menu/resume_btn_pressed.png";
        public static readonly string RestartBtn = "Content/Menu/restart_btn.png";
        public static readonly string RestartBtnPressed = "Content/Menu/restart_btn_pressed.png";
        public static readonly string SelectionBtn = "Content/Menu/selection_btn.png";
        public static readonly string SelectionBtnPressed = "Content/Menu/selection_btn_pressed.png";

        public static readonly string LevelSelectBgPanel = "Content/Menu/level_select_bg.png";
        public static readonly string CloseBtn = "Content/Menu/close_btn.png";
        public static readonly string CloseBtnPressed = "Content/Menu/close_btn_pressed.png";
        public static readonly string LevelBtn = "Content/Menu/level_btn.png";
        public static readonly string LevelBtnPressed = "Content/Menu/level_btn_pressed.png";
        public static readonly string LevelBtnLocked = "Content/Menu/level_btn_locked.png";
        public static readonly string StartLevelActive = "Content/Menu/star_active.png";
        public static readonly string StartLevelUnactive = "Content/Menu/star_unactive.png";
        public static readonly string StartLevelUnactiveLocked = "Content/Menu/star_unactive_locked.png";

        // Music
        public static readonly string BgMusic = "Content/Music/Ozzed_-_A_Well_Worked_Analogy.mp3";

        //Sounds
        public static readonly string CoinSound ="Content/Sound/coin.wav";
        public static readonly string ContactSound ="Content/Sound/contact.wav";
        public static readonly string CreateDropSound ="Content/Sound/crateDrop.wav";
        public static readonly string CrashSound ="Content/Sound/crash.wav";
        public static readonly string JumSound ="Content/Sound/jump.wav";
        public static readonly string VictorySound ="Content/Sound/victory.wav";
        public static readonly string ButtonSound = "Content/Sound/button.wav";
    }
}
