#region File Description
//-----------------------------------------------------------------------------
// OrbitRabbits
//
// Quickstarter for Wave University Tour 2014.
// Author: Wave Engine Team
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using OrbitRabbits.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Media;
using WaveEngine.Framework.Services;
using WaveEngine.Framework.Sound; 
#endregion

namespace OrbitRabbits.Managers
{
    public class SoundsManager
    {
        /// <summary>
        /// List sounds
        /// </summary>
        public enum SOUNDS
        {
            Impulse,
            Explosion,     
            Click,
            Fall,
            Start,
            brokenGlass,
            Last
        };

        private SoundInfo[] sounds;
        private SoundBank soundsBank;
        private float defaultVolume = 1.0f;
        private static SoundsManager instance;

        public static SoundsManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SoundsManager();
                }

                return instance;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SoundsManager" /> class.
        /// </summary>
        private SoundsManager()
        {
            soundsBank = new SoundBank();
            soundsBank.MaxConcurrentSounds = 10;
            WaveServices.SoundPlayer.RegisterSoundBank(soundsBank);

            this.sounds = new SoundInfo[(int)SOUNDS.Last];

            this.LoadSound(SOUNDS.Impulse, Directories.SoundsPath + "impulse.wpk");
            this.LoadSound(SOUNDS.Explosion, Directories.SoundsPath + "explosion.wpk");
            this.LoadSound(SOUNDS.Click, Directories.SoundsPath + "click.wpk");
            this.LoadSound(SOUNDS.Start, Directories.SoundsPath + "start.wpk");
            this.LoadSound(SOUNDS.Fall, Directories.SoundsPath + "fall.wpk");
            this.LoadSound(SOUNDS.brokenGlass, Directories.SoundsPath + "brokenGlass.wpk");
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
            this.soundsBank.AddWithouthLoad(this.sounds[soundIndex]);
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
