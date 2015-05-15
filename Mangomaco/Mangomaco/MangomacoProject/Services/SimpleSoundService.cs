#region Using Statements
using System.Collections.Generic;
using WaveEngine.Common;
using WaveEngine.Common.Media;
using WaveEngine.Framework.Services;
using WaveEngine.Framework.Sound;
#endregion

namespace MangomacoProject.Services
{
    /// <summary>
    /// Simple Sound Service
    /// </summary>
    public class SimpleSoundService : Service
    {
        /// <summary>
        /// List sounds
        /// </summary>
        public enum SoundType
        {
            Coin = 0,
            Contact,
            Crash,
            CrateDrop,
            Jump,
            Victory,
            Button,
        }

        /// <summary>
        /// The sound player
        /// </summary>
        private SoundPlayer soundPlayer;

        /// <summary>
        /// The bank
        /// </summary>
        private SoundBank bank;

        /// <summary>
        /// The sounds
        /// </summary>
        private Dictionary<SoundType, SoundInfo> sounds;

        /// <summary>
        /// The muted
        /// </summary>
        private bool muted;

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="SimpleSoundService" /> is mute.
        /// </summary>
        public bool Mute
        {
            get { return this.muted; }
            set
            {
                this.muted = value;
                WaveServices.MusicPlayer.IsMuted = value;

                if (this.muted)
                {
                    WaveServices.SoundPlayer.StopAllSounds();
                }
            }
        }

        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleSoundService" /> class.
        /// </summary>
        protected override void Initialize()
        {
            this.soundPlayer = WaveServices.SoundPlayer;

            // fill sound info
            sounds = new Dictionary<SoundType, SoundInfo>();
            sounds[SoundType.Coin] = new SoundInfo(Resources.CoinSound);
            sounds[SoundType.Contact] = new SoundInfo(Resources.ContactSound);
            sounds[SoundType.CrateDrop] = new SoundInfo(Resources.CreateDropSound);
            sounds[SoundType.Crash] = new SoundInfo(Resources.CrashSound);
            sounds[SoundType.Jump] = new SoundInfo(Resources.JumSound);
            sounds[SoundType.Victory] = new SoundInfo(Resources.VictorySound);
            sounds[SoundType.Button] = new SoundInfo(Resources.ButtonSound);

            this.bank = new SoundBank();
            this.soundPlayer.RegisterSoundBank(bank);
            foreach (var item in this.sounds)
            {
                this.bank.Add(item.Value);
            }
        }

        /// <summary>
        /// Allow to execute custom logic during the finalization of this instance.
        /// </summary>
        protected override void Terminate()
        {
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Plays the sound.
        /// </summary>
        /// <param name="soundType">The sound.</param>
        /// <param name="volume">The volume.</param>
        /// /// <param name="loop">if set to <c>true</c> [loop].</param>
        /// <returns>Sound instance</returns>
        public SoundInstance PlaySound(SoundType soundType, float volume = 1f, bool loop = false)
        {
            if (muted)
            {
                return null;
            }

            return this.soundPlayer.Play(this.sounds[soundType], volume, loop);
        }
        #endregion
    }
}
