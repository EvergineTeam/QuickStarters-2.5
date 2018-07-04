#region Using Statements
using FlyingKite.Managers;
using FlyingKite.Scenes;
using System;
using WaveEngine.Common;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Media;
using WaveEngine.Components;
using WaveEngine.Framework;
using WaveEngine.Framework.Services;
#endregion

namespace FlyingKite
{
    public class Game : WaveEngine.Framework.Game
    {
        public override void Initialize(IApplication application)
        {
            base.Initialize(application);

            this.Load(WaveContent.GameInfo);

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
            
            // Play background music
            WaveServices.MusicPlayer.Play(new MusicInfo(WaveContent.Assets.Audio.bg_music_mp3));
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
