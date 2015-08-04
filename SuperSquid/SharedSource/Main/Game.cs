#region Using Statements
using SuperSquid.Commons;
using SuperSquid.Managers;
using SuperSquid.Scenes;
using System;
using WaveEngine.Common;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Input;
using WaveEngine.Common.Media;
using WaveEngine.Components;
using WaveEngine.Framework;
using WaveEngine.Framework.Services;
#endregion

namespace SuperSquid
{
    public class Game : WaveEngine.Framework.Game
    {
        public override void Initialize(IApplication application)
        {
            base.Initialize(application);

            application.Adapter.DefaultOrientation = DisplayOrientation.Portrait;
            application.Adapter.SupportedOrientations = DisplayOrientation.Portrait;

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

            // Use ViewportManager to ensure scaling in all devices
            ViewportManager vm = WaveServices.ViewportManager;
            vm.Activate(768, 1024, ViewportManager.StretchMode.Uniform);

            var backContext = new ScreenContext("BackContext", new BackgroundScene())
            {
                Behavior = ScreenContextBehaviors.DrawInBackground | ScreenContextBehaviors.UpdateInBackground
            };

            var mainContext = new ScreenContext(new MainMenuScene());


            WaveServices.ScreenContextManager.Push(backContext);
            WaveServices.ScreenContextManager.Push(mainContext);

            // Play music
            WaveServices.MusicPlayer.Play(new MusicInfo(Directories.SoundsPath + "bg_music.mp3"));
            WaveServices.MusicPlayer.Volume = 1.0f;
            WaveServices.MusicPlayer.IsRepeat = true;
        }
    }
}
