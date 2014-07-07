#region Using Statements
using DeepSpaceProject.Managers;
using System;
using WaveEngine.Common;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Media;
using WaveEngine.Components;
using WaveEngine.Framework;
using WaveEngine.Framework.Services;
#endregion

namespace DeepSpaceProject
{
    public class Game : WaveEngine.Framework.Game
    {
        public override void Initialize(IApplication application)
        {
            base.Initialize(application);

            // Load storage game data
            GameStorage gameStorage;
            if (WaveServices.Storage.Exists<GameStorage>())
            {
                gameStorage = WaveServices.Storage.Read<GameStorage>();
            }
            else
            {
                gameStorage = new GameStorage();
            }

            Catalog.RegisterItem(gameStorage);

            // Register the SoundManager service
            WaveServices.RegisterService<SoundManager>(new SoundManager());

            WaveServices.ViewportManager.Activate(768, 1280, ViewportManager.StretchMode.Uniform);

            var gameContext = new ScreenContext("GamePlayContext", new GamePlayScene())
            {
                Behavior = ScreenContextBehaviors.DrawInBackground | ScreenContextBehaviors.UpdateInBackground
            };
            var introContext = new ScreenContext(new IntroScene());
            WaveServices.ScreenContextManager.Push(gameContext);
            WaveServices.ScreenContextManager.Push(introContext);

            // Play background music
            WaveServices.MusicPlayer.Play(new MusicInfo("Content/music.mp3"));
            WaveServices.MusicPlayer.Volume = 0.3f;
            WaveServices.MusicPlayer.IsRepeat = true;
        }
    }
}
