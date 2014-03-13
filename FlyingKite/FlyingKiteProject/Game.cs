#region Using Statements
using FlyingKite.Managers;
using FlyingKiteProject.Managers;
using FlyingKiteProject.Resources;
using FlyingKiteProject.Scenes;
using System;
using WaveEngine.Common;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Media;
using WaveEngine.Components;
using WaveEngine.Framework;
using WaveEngine.Framework.Services;
#endregion

namespace FlyingKiteProject
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

            Catalog.RegisterItem<GameStorage>(gameStorage);

            // Register the SoundManager service
            WaveServices.RegisterService<SoundManager>(new SoundManager());

            // Use ViewportManager to ensure scaling in all devices
            WaveServices.ViewportManager.Activate(
                1024,
                768,
                ViewportManager.StretchMode.Uniform);

            // Play background music
            WaveServices.MusicPlayer.Play(new MusicInfo(Sounds.BG_MUSIC));
            WaveServices.MusicPlayer.IsRepeat = true;

            // GameBackContext is visible always at the background. 
            // For this reason the behavior is set to Draw and Update when the scene is in background.
            var backContext = new ScreenContext("GameBackContext", new GameScene());
            backContext.Behavior = ScreenContextBehaviors.DrawInBackground | ScreenContextBehaviors.UpdateInBackground;

            //On init show the Main menu
            WaveServices.ScreenContextManager.Push(backContext);
            WaveServices.ScreenContextManager.Push(new ScreenContext("MenuContext", new MenuScene()));
        }
    }
}
