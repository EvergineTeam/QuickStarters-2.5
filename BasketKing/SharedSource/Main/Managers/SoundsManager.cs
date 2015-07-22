#region Using Statements
using BasketKing.Commons;
using WaveEngine.Common.Media;
using WaveEngine.Framework.Services;
using WaveEngine.Framework.Sound;
#endregion

namespace BasketKing.Managers
{
    public class SoundsManager
    {
        /// <summary>
        /// List sounds
        /// </summary>
        public enum SOUNDS
        {
            Hoop,
            Net,
            Tablet,
            Ready,
            HurryUp,
            Go,
            TimeOver,
            Button,
            Combo,
            Record,
            Digit,
            Last
        };

        public static readonly SoundsManager Instance = new SoundsManager();

        private SoundInfo[] sounds;
        private SoundBank soundsBank;
        private float defaultVolume = 1.0f;
        private bool muted;

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="SoundsManager" /> is mute.
        /// </summary>
        public bool Mute
        {
            get { return muted; }
            set
            {
                muted = value;
                WaveServices.MusicPlayer.IsMuted = value;

                if (this.muted)
                {
                    WaveServices.SoundPlayer.StopAllSounds();
                }
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="SoundsManager" /> class.
        /// </summary>
        private SoundsManager()
        {
            soundsBank = new SoundBank();
            soundsBank.MaxConcurrentSounds = 10;
            WaveServices.SoundPlayer.RegisterSoundBank(soundsBank);

            this.sounds = new SoundInfo[(int)SOUNDS.Last];

            this.LoadSound(SOUNDS.Hoop, Directories.Sounds + "hoop.wpk");
            this.LoadSound(SOUNDS.Net, Directories.Sounds + "net.wpk");
            this.LoadSound(SOUNDS.Tablet, Directories.Sounds + "tablet.wpk");
            this.LoadSound(SOUNDS.Ready, Directories.Sounds + "ready.wpk");
            this.LoadSound(SOUNDS.HurryUp, Directories.Sounds + "hurryUp.wpk");
            this.LoadSound(SOUNDS.Go, Directories.Sounds + "go.wpk");
            this.LoadSound(SOUNDS.TimeOver, Directories.Sounds + "timeOver.wpk");
            this.LoadSound(SOUNDS.Button, Directories.Sounds + "button.wpk");
            this.LoadSound(SOUNDS.Combo, Directories.Sounds + "yeah.wpk");
            this.LoadSound(SOUNDS.Record, Directories.Sounds + "record.wpk");
            this.LoadSound(SOUNDS.Digit, Directories.Sounds + "digit.wpk");
        }

        /// <summary>
        /// Loads the sound.
        /// </summary>
        /// <param name="sound">The sound.</param>
        /// <param name="file">The file.</param>
        private void LoadSound(SOUNDS sound, string file)
        {
            int soundIndex = (int)sound;

            this.sounds[soundIndex] = new SoundInfo(file);
            this.soundsBank.Add(this.sounds[soundIndex]);
        }

        /// <summary>
        /// Plays the sound.
        /// </summary>
        /// <param name="sound">The sound.</param>
        /// <returns></returns>
        public SoundInstance PlaySound(SOUNDS sound)
        {
            return this.PlaySound(sound, defaultVolume, false);
        }

        /// <summary>
        /// Plays the sound.
        /// </summary>
        /// <param name="sound">The sound.</param>
        /// <param name="volume">The volume.</param>
        /// <returns></returns>
        public SoundInstance PlaySound(SOUNDS sound, float volume)
        {
            return this.PlaySound(sound, volume, false);
        }

        /// <summary>
        /// Plays the sound.
        /// </summary>
        /// <param name="sound">The sound.</param>
        /// <param name="loop">if set to <c>true</c> [loop].</param>
        /// <returns></returns>
        public SoundInstance PlaySound(SOUNDS sound, bool loop)
        {
            return this.PlaySound(sound, defaultVolume, loop);
        }

        /// <summary>
        /// Plays the sound.
        /// </summary>
        /// <param name="sound">The sound.</param>
        /// <param name="volume">The volume.</param>
        /// <param name="loop">if set to <c>true</c> [loop].</param>
        /// <returns></returns>
        public SoundInstance PlaySound(SOUNDS sound, float volume, bool loop)
        {
            if (muted)
            {
                return null;
            }

            int soundIndex = (int)sound;
            if (soundIndex >= 0
                && soundIndex < this.sounds.Length
                && this.sounds[soundIndex] != null
                )
            {
                return WaveServices.SoundPlayer.Play(this.sounds[soundIndex], volume, loop);
            }
            return null;
        }
    }
}
