using WaveEngine.Framework.Services;
using System;
using WaveEngine.Common.Math;
using WaveEngine.Common;
using WaveEngine.Framework.Sound;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using WaveEngine.Common.Media;
using System.Diagnostics;
using P2PTank3D;

namespace P2PTank.Services
{
    /// <summary>
    /// Audio Service class.
    /// </summary>
    public class AudioService : UpdatableService
    {
        private const float MUSIC_VOLUME = 0.2f;
        private const float SFX_VOLUME = 0.5f;
        private const float MUSIC_FADEIN_SECONDS = 0.5f;

        private bool muteSFX;

        private SoundInfo[] sounds;
        private Dictionary<string, string> audioPaths;
        private SoundBank soundsBank;
        private Audio.Music? currentMusic;

        private MusicPlayer musicPlayer;
        private SoundPlayer soundPlayer;
        private TimeSpan musicFadeTimer;

        #region Properties

        /// <summary>
        /// Gets the music volume.
        /// </summary>
        public float MusicVolume
        {
            get
            {
                return this.musicPlayer.Volume;
            }

            set
            {
                this.musicPlayer.Volume = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [mute music].
        /// </summary>
        public bool MuteMusic
        {
            get
            {
                return this.musicPlayer.IsMuted;
            }

            set
            {
                this.musicPlayer.IsMuted = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [mute SFX].
        /// </summary>
        public bool MuteSFX
        {
            get
            {
                return this.muteSFX;
            }

            set
            {
                this.muteSFX = value;

                //foreach (var sound in this.uniqueLoopedSounds.Values)
                //{
                //    sound.Volume = this.muteSFX ? 0f : SFX_VOLUME;
                //}
            }
        }

        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="SoundsManager" /> class.
        /// </summary>
        protected override void Initialize()
        {
            this.musicPlayer = WaveServices.MusicPlayer;
            this.soundPlayer = WaveServices.SoundPlayer;

            this.soundsBank = new SoundBank();
            this.soundsBank.MaxConcurrentSounds = 5;
            this.soundPlayer.RegisterSoundBank(this.soundsBank);

            // Create SoundInfo array
            this.sounds = new SoundInfo[Enum.GetValues(typeof(Audio.Sfx)).Length];

            this.LoadAllSounds();
        }

        private string GetSoundPath(Audio.Sfx sfx)
        {
            string res = string.Empty;
            switch (sfx)
            {
                case Audio.Sfx.Zap_wav:
                    res = WaveContent.Assets.Sounds.Zap_wav;
                    break;
                case Audio.Sfx.Gun_wav:
                    res = WaveContent.Assets.Sounds.Gun_wav;
                    break;
                case Audio.Sfx.Explosion_wav:
                    res = WaveContent.Assets.Sounds.Explosion_wav;
                    break;
                case Audio.Sfx.BulletCollision_wav:
                    res = WaveContent.Assets.Sounds.BulletCollision_wav;
                    break;
                case Audio.Sfx.SpawnPowerUp_wav:
                    res = WaveContent.Assets.Sounds.SpawnPowerUp_wav;
                    break;
                case Audio.Sfx.PowerUp_wav:
                    res = WaveContent.Assets.Sounds.PowerUp_wav;
                    break;
                default:
                    res = WaveContent.Assets.Sounds.PowerUp_wav;
                    break;
            }

            return res;
        }

        private string GetMusicPath(Audio.Music music)
        {
            string res = string.Empty;
            switch (music)
            {
                case Audio.Music.Background_mp3:
                    res = WaveContent.Assets.Sounds.Background_mp3;
                    break;
                default:
                    res = WaveContent.Assets.Sounds.Background_mp3;
                    break;
            }

            return res;
        }

        /// <summary>
        /// Loads all sounds.
        /// </summary>
        public void LoadAllSounds()
        {
            //Load all Sounds
            foreach (var item in Enum.GetValues(typeof(Audio.Sfx)))
            {
                //WaveContent.Assets.Sounds.Zap_wav
                SoundInfo a = new SoundInfo(this.GetSoundPath((Audio.Sfx)item));
                this.soundsBank.Add(a);
            }
        }

        private void SearchMusicAndSounds(Type contentType)
        {
            // Search all fields
            FieldInfo[] fields = contentType.GetFields();
            foreach (FieldInfo field in fields)
            {
                if (field.FieldType == typeof(string))
                {
                    string fieldValue = (string)field.GetValue(null);
                    if (!string.IsNullOrEmpty(fieldValue)
                         && (Path.GetExtension(fieldValue) == ".wav" || Path.GetExtension(fieldValue) == ".mp3")
                        )
                    {
                        this.audioPaths.Add(field.Name, fieldValue);
                    }
                }
            }
#if WINDOWS_UWP
            // Search child classes
            Type[] types = contentType.GetNestedTypes(BindingFlags.Public);
#else
            // Search child classes
            Type[] types = contentType.GetNestedTypes();
#endif
            foreach (Type type in types)
            {
                this.SearchMusicAndSounds(type);
            }

        }

        #endregion

        #region Public Methods       

        /// <summary>
        /// Plays the sound.
        /// </summary>
        /// <param name="sound">The sound.</param>
        /// <param name="volume">The volume.</param>
        /// <param name="loop">if set to <c>true</c> [loop].</param>
        /// <returns></returns>
        public void Play(Audio.Sfx sound, bool loop = false)
        {
            if (!this.muteSFX)
            {
                this.InternalPlaySound(sound, loop);
            }
        }

        public void PlayRandom(params Audio.Sfx[] sounds)
        {
            if (!this.muteSFX && sounds != null && sounds.Length > 0)
            {
                var index = WaveServices.Random.Next(sounds.Length);
                this.InternalPlaySound(sounds[index], false);
            }
        }

        /// <summary>
        /// Internals the play sound.
        /// </summary>
        /// <param name="sound">The sound.</param>
        /// <param name="loop">if set to <c>true</c> [loop].</param>
        /// <returns></returns>
        private SoundInstance InternalPlaySound(Enum sound, bool loop)
        {
            try
            {
                int soundIndex = Convert.ToInt32(sound);
                var instance = this.soundPlayer.Play(this.sounds[soundIndex], SFX_VOLUME, loop);
                if (instance != null && this.muteSFX)
                {
                    instance.Volume = 0f;
                }

                return instance;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);

                return null;
            }
        }

        /// <summary>
        /// Plays the specified music.
        /// </summary>
        /// <param name="music">The music.</param>
        public void Play(Audio.Music music, float volume = MUSIC_VOLUME)
        {
            this.musicPlayer.Volume = volume;
            this.musicPlayer.IsRepeat = true;

            if (this.currentMusic == null || this.currentMusic != music)
            {
                this.currentMusic = music;

                var musicInfo = new MusicInfo(this.GetMusicPath(music));
                this.musicPlayer.Play(musicInfo);
            }

            this.musicPlayer.Volume = volume;
            this.musicPlayer.IsRepeat = true;
        }

        #endregion

        /// <summary>
        /// Terminates this instance.
        /// </summary>
        protected override void Terminate()
        {
            this.sounds = null;
            this.musicPlayer.Stop();
            this.soundPlayer.StopAllSounds();
        }

        /// <summary>
        /// Updates the specified game time.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public override void Update(TimeSpan gameTime)
        {
            if (this.musicFadeTimer.TotalSeconds > 0)
            {
                this.musicFadeTimer -= gameTime;

                if (this.musicFadeTimer.TotalSeconds <= 0)
                {
                    this.musicFadeTimer = TimeSpan.Zero;
                    this.MusicVolume = MUSIC_VOLUME;
                }
                else
                {
                    var amount = (float)(1 - (this.musicFadeTimer.TotalSeconds / MUSIC_FADEIN_SECONDS));
                    this.MusicVolume = MathHelper.Lerp(MUSIC_VOLUME * 0.5f, MUSIC_VOLUME, amount);
                }
            }
        }
    }
}
