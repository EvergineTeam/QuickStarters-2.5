#region File Description
//-----------------------------------------------------------------------------
// Super Squid
//
// Quickstarter for Wave University Tour 2014.
// Author: Wave Engine Team
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using SuperSquidProject.Commons;
using SuperSquidProject.Managers;
using SuperSquidProject.Scenes;
using System;
using WaveEngine.Common;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Input;
using WaveEngine.Common.Media;
using WaveEngine.Components;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
#endregion

namespace SuperSquidProject
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

            // Set portrait orientation in WP
            if (WaveServices.Platform.PlatformType == PlatformType.WindowsPhone)
            {
                application.Adapter.DefaultOrientation = DisplayOrientation.Portrait;
                application.Adapter.SupportedOrientations = DisplayOrientation.Portrait;
            }

            // Use ViewportManager to ensure scaling in all devices
            ViewportManager vm = WaveServices.ViewportManager;
            vm.Activate(768, 1024, ViewportManager.StretchMode.Uniform);

            // Load heavy assets to avoid breaks during gameplay
            this.LoadHeavyAssets();

            var backContext = new ScreenContext("BackContext", new BackgroundScene())
            {
                Behavior = ScreenContextBehaviors.DrawInBackground | ScreenContextBehaviors.UpdateInBackground
            };
            var mainContext = new ScreenContext(new MainMenuScene());
            WaveServices.ScreenContextManager.Push(backContext);      
            WaveServices.ScreenContextManager.Push(mainContext);            
        }

        /// <summary>
        /// Load heavy assets in global storage
        /// </summary>
        private void LoadHeavyAssets()
        {
            WaveServices.Assets.Global.LoadAsset<Texture2D>(Directories.TexturePath + "centralRock.wpk");
            WaveServices.Assets.Global.LoadAsset<Texture2D>(Directories.TexturePath + "centralRockCollider.wpk");
            WaveServices.Assets.Global.LoadAsset<Texture2D>(Directories.TexturePath + "game.wpk");
            WaveServices.Assets.Global.LoadAsset<Texture2D>(Directories.TexturePath + "leftRock.wpk");
            WaveServices.Assets.Global.LoadAsset<Texture2D>(Directories.TexturePath + "leftRockCollider.wpk");
            WaveServices.Assets.Global.LoadAsset<Texture2D>(Directories.TexturePath + "rightRock.wpk");
            WaveServices.Assets.Global.LoadAsset<Texture2D>(Directories.TexturePath + "rightRockCollider.wpk");
            WaveServices.Assets.Global.LoadAsset<Texture2D>(Directories.TexturePath + "jellyFishSpriteSheet.wpk");
            WaveServices.Assets.Global.LoadAsset<Texture2D>(Directories.TexturePath + "jellyFishLittleSpriteSheet.wpk");
            WaveServices.Assets.Global.LoadAsset<Texture2D>(Directories.TexturePath + "seaweedsSpriteSheet.wpk");
            WaveServices.Assets.Global.LoadAsset<Texture2D>(Directories.TexturePath + "squidSpriteSheet.wpk");
            WaveServices.Assets.Global.LoadAsset<Texture2D>(Directories.TexturePath + "squid.wpk");
            WaveServices.Assets.Global.LoadAsset<Texture2D>(Directories.TexturePath + "starfish.wpk");
            WaveServices.Assets.Global.LoadAsset<Texture2D>(Directories.TexturePath + "super.wpk");
        }
    }
}
