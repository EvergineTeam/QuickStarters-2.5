#region Using Statements
using SurvivorNinja.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Media;
using WaveEngine.Framework.Services;
using WaveEngine.Framework.Sound; 
#endregion

namespace SurvivorNinja.Managers
{
    public class SoundsManager
    {
        /// <summary>
        /// List sounds
        /// </summary>
        public enum SOUNDS
        {
            Shoot,
            Impact,
            EnemyDead,
            playerHurt,
            Ready,
            GameOver,
            Last
        };

        public static readonly SoundsManager Instance = new SoundsManager();

        private SoundInfo[] sounds;
        private SoundBank soundsBank;
        private float defaultVolume = 1.0f;

        /// <summary>
        /// Initializes a new instance of the <see cref="SoundsManager" /> class.
        /// </summary>
        private SoundsManager()
        {
            soundsBank = new SoundBank();
            soundsBank.MaxConcurrentSounds = 10;
            WaveServices.SoundPlayer.RegisterSoundBank(soundsBank);

            this.sounds = new SoundInfo[(int)SOUNDS.Last];

            this.LoadSound(SOUNDS.Shoot, WaveContent.Assets.Sounds.shoot_wav);
            this.LoadSound(SOUNDS.Impact, WaveContent.Assets.Sounds.impact_wav);
            this.LoadSound(SOUNDS.EnemyDead, WaveContent.Assets.Sounds.enemyDead_wav);
            this.LoadSound(SOUNDS.playerHurt, WaveContent.Assets.Sounds.playerHurt_wav);
            this.LoadSound(SOUNDS.Ready, WaveContent.Assets.Sounds.ready_wav);
            this.LoadSound(SOUNDS.GameOver, WaveContent.Assets.Sounds.gameOver_wav);
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
