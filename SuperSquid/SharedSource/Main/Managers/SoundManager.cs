#region File Description
//-----------------------------------------------------------------------------
// Super Squid
//
// Quickstarter for Wave University Tour 2014.
// Author: Wave Engine Team
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using SuperSquid.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common;
using WaveEngine.Common.Media;
using WaveEngine.Framework.Services;
using WaveEngine.Framework.Sound;
#endregion

namespace SuperSquid.Managers
{
    public class SoundManager : Service
    {
        /// <summary>
        /// List sounds
        /// </summary>
        public enum SOUNDS
        {
            SquidSwim1,
            SquidSwim2,
            JellyCrash,
            RockCrash,
            Star,
            Last
        };

        private SoundInfo[] sounds;
        private SoundBank soundsBank;
        private float defaultVolume = 1.0f;

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

            this.LoadSound(SOUNDS.SquidSwim1, Directories.SoundsPath + "squid_swim1.wpk");
            this.LoadSound(SOUNDS.SquidSwim2, Directories.SoundsPath + "squid_swim2.wpk");
            this.LoadSound(SOUNDS.JellyCrash, Directories.SoundsPath + "jellyCrash.wpk");
            this.LoadSound(SOUNDS.RockCrash, Directories.SoundsPath + "rockCrash.wpk");
            this.LoadSound(SOUNDS.Star, Directories.SoundsPath + "star.wpk");
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
            if (soundIndex >= 0
                && soundIndex < this.sounds.Length
                && this.sounds[soundIndex] != null
                )
            {
                return WaveServices.SoundPlayer.Play(this.sounds[soundIndex], volume, loop);
            }
            return null;
        }
        #endregion

        protected override void Terminate()
        {
        }
    }
}
