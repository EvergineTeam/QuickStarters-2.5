#region Using Statements
using OrbitRabbits.Commons;
using OrbitRabbits.Managers;
using System;
using WaveEngine.Common;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Input;
using WaveEngine.Common.Media;
using WaveEngine.Components;
using WaveEngine.Framework;
using WaveEngine.Framework.Services;
#endregion

namespace OrbitRabbits
{
    public class Game : WaveEngine.Framework.Game
    {
        public override void Initialize(IApplication application)
        {
            base.Initialize(application);

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

            application.Adapter.DefaultOrientation = DisplayOrientation.Portrait;
            application.Adapter.SupportedOrientations = DisplayOrientation.Portrait;

            ViewportManager vm = WaveServices.ViewportManager;
            vm.Activate(768, 1024, ViewportManager.StretchMode.Uniform);


			ScreenContext screenContext = new ScreenContext(new MainMenuScene());

            // Play background music
            WaveServices.MusicPlayer.Play(new MusicInfo(WaveContent.Assets.Sounds.bg_music_mp3)); 
            WaveServices.MusicPlayer.IsRepeat = true;

			WaveServices.ScreenContextManager.To(screenContext);
        }

        /// <summary>
        /// Called when [deactivated].
        /// </summary>
        public override void OnDeactivated()
        {
            base.OnDeactivated();

            // Save game storage
            GameStorage gameStorage = Catalog.GetItem<GameStorage>();
            WaveServices.Storage.Write<GameStorage>(gameStorage);
        }
    }
}
