using System.Collections.Generic;
using WaveEngine.Common;
using WaveEngine.Common.Media;
using WaveEngine.Framework.Services;
using WaveEngine.Framework.Sound;
using System;

namespace Match3.Services.Audio
{
    public class AudioPlayer : UpdatableService
    {
        private bool isMuted;

        private List<SoundInstance> runningSoundInstances;
        private SoundInfo[] sounds;
        private SoundBank soundsBank;

        private MusicPlayer musicPlayer;
        private Songs currentSong;
        private Dictionary<Songs, MusicInfo> songs;

        public bool IsMuted
        {
            get { return this.isMuted; }
            set
            {
                this.isMuted = value;
                WaveServices.MusicPlayer.IsMuted = this.isMuted;
            }
        }

        public float Speed { get; set; }

        protected override void Initialize()
        {
            this.Speed = 1;

            this.musicPlayer = WaveServices.MusicPlayer;

            this.soundsBank = new SoundBank();
            this.soundsBank.MaxConcurrentSounds = 10;
            WaveServices.SoundPlayer.RegisterSoundBank(soundsBank);

            this.runningSoundInstances = new List<SoundInstance>();

            var soundCount = Enum.GetValues(typeof(Sounds)).Length;
            this.sounds = new SoundInfo[soundCount];
            this.LoadSound(Sounds.Button, WaveContent.Assets.Audio.Button_wav);
            this.LoadSound(Sounds.ComboAppear, WaveContent.Assets.Audio.ComboAppear_wav);
            this.LoadSound(Sounds.ComboAppear2, WaveContent.Assets.Audio.ComboAppear2_wav);
            this.LoadSound(Sounds.CountDown, WaveContent.Assets.Audio.Countdown_wav);
            this.LoadSound(Sounds.InvalidMovement, WaveContent.Assets.Audio.InvalidMovement_wav);
            this.LoadSound(Sounds.ValidMovement, WaveContent.Assets.Audio.ValidMovement_wav);
            this.LoadSound(Sounds.CandyBoom, WaveContent.Assets.Audio.CandyBoom_wav);

            var songCount = Enum.GetValues(typeof(Songs)).Length;
            this.songs = new Dictionary<Songs, MusicInfo>(songCount);
            this.songs.Add(Songs.Menu, new MusicInfo(WaveContent.Assets.Audio.bgMusic_mp3));
        }

        protected override void Terminate()
        {
            this.runningSoundInstances.Clear();
        }

        /// <summary>
        /// Loads the sound.
        /// </summary>
        /// <param name="sound">The sound.</param>
        /// <param name="file">The file.</param>
        private void LoadSound(Sounds sound, string file)
        {
            int soundIndex = (int)sound;

            this.sounds[soundIndex] = new SoundInfo(file);
            this.soundsBank.Add(this.sounds[soundIndex]);
        }

        private SoundInfo GetSoundInfo(Sounds sound)
        {
            SoundInfo result = null;

            var soundIndex = (int)sound;

            if (soundIndex >= 0 &&
                soundIndex < this.sounds.Length)
            {
                result = this.sounds[soundIndex];
            }

            return result;
        }

        private void UpdateInstanceParams(SoundInstance sound)
        {
            sound.Volume = this.IsMuted ? 0 : 1;
            sound.Pitch = this.Speed - 1;
        }

        public SoundInstance PlaySound(Sounds sound, float volume = 1f, bool loop = false)
        {
            SoundInstance result = null;

            var soundInfo = this.GetSoundInfo(sound);

            if (soundInfo != null)
            {
                result = WaveServices.SoundPlayer.Play(soundInfo, volume, loop);

                if (result != null)
                {
                    this.UpdateInstanceParams(result);
                    this.runningSoundInstances.Add(result);
                }
            }

            return result;
        }

        public void PlayMusic(Songs song)
        {
            if (this.currentSong != song)
            {
                var musicInfo = this.songs[song];

                this.currentSong = song;

                this.musicPlayer.Play(musicInfo);
                this.musicPlayer.Volume = 0.5f;
                this.musicPlayer.IsRepeat = true;
            }
        }

        public void StopAllSounds()
        {
            foreach (var instance in this.runningSoundInstances)
            {
                if (instance.State != SoundState.Paused)
                {
                    instance.Stop();
                }
            }
        }

        public void StopAll()
        {
            WaveServices.MusicPlayer.Stop();
            this.StopAllSounds();

            this.currentSong = Songs.None;
        }

        public override void Update(TimeSpan gameTime)
        {
            for (int i = this.runningSoundInstances.Count - 1; i >= 0; i--)
            {
                var sound = this.runningSoundInstances[i];

                if (sound.State == SoundState.Stopped)
                {
                    this.runningSoundInstances.RemoveAt(i);
                }
                else
                {
                    this.UpdateInstanceParams(sound);
                }
            }
        }
    }
}
