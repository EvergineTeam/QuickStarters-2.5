using WaveEngine.Common;
using WaveEngine.Common.Media;
using WaveEngine.Framework.Services;
using WaveEngine.Framework.Sound;

namespace DeepSpace.Managers
{
    public class SoundManager : Service
    {
        /// <summary>
        /// List sounds
        /// </summary>
        public enum SOUNDS
        {
            Blast0,
            Blast1,
            Blast2,
            Explode0,
            Explode1,
            Explode2,
            Last
        };

        private SoundInfo[] sounds;
        private SoundBank soundsBank;
        private float defaultVolume = 0.5f;

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="SoundsManager" /> class.
        /// </summary>
        protected override void Initialize()
        {
            soundsBank = new SoundBank();
            soundsBank.MaxConcurrentSounds = 10;
            WaveServices.SoundPlayer.RegisterSoundBank(soundsBank);

            this.sounds = new SoundInfo[(int)SOUNDS.Last];

            this.LoadSound(SOUNDS.Blast0, WaveContent.Assets.blast0_wav);
            this.LoadSound(SOUNDS.Blast1, WaveContent.Assets.blast1_wav);
            this.LoadSound(SOUNDS.Blast2, WaveContent.Assets.blast2_wav);

            this.LoadSound(SOUNDS.Explode0, WaveContent.Assets.explode0_wav);
            this.LoadSound(SOUNDS.Explode1, WaveContent.Assets.explode1_wav);
            this.LoadSound(SOUNDS.Explode2, WaveContent.Assets.explode2_wav);
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
        #endregion

        #region Public Methods
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
            int soundIndex = (int)sound;
            if (soundIndex >= 0 && soundIndex < this.sounds.Length && this.sounds[soundIndex] != null)
            {
                return WaveServices.SoundPlayer.Play(this.sounds[soundIndex], volume, loop);
            }
            return null;
        }
        #endregion

        protected override void Terminate()
        {
            foreach (var sound in this.sounds)
            {
                this.soundsBank.Remove(sound);
            }
        }
    }
}
