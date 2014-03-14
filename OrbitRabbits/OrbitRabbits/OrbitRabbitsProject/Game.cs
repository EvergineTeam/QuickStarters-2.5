#region File Description
//-----------------------------------------------------------------------------
// OrbitRabbits
//
// Quickstarter for Wave University Tour 2014.
// Author: Wave Engine Team
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using OrbitRabbitsProject.Commons;
using OrbitRabbitsProject.Managers;
using System;
using WaveEngine.Common;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Input;
using WaveEngine.Common.Media;
using WaveEngine.Components;
using WaveEngine.Framework;
using WaveEngine.Framework.Services;
#endregion

namespace OrbitRabbitsProject
{
    public class Game : WaveEngine.Framework.Game
    {
        /// <summary>
        /// Initializes the specified adapter.
        /// </summary>
        /// <param name="application">The application.</param>
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

            // Play background music
            WaveServices.MusicPlayer.Play(new MusicInfo(Directories.SoundsPath + "bg_music.mp3"));
            WaveServices.MusicPlayer.IsRepeat = true;

            WaveServices.ScreenContextManager.To(new ScreenContext(new MainMenuScene()));
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
