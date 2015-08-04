using System;
using System.Collections.Generic;
using WaveEngine.Framework.Services;
using System.Threading;
using Android.App;
using Android.Content.PM;
using Android.Views;

namespace SuperSquid
{
    [Activity(Label = "SuperSquid",
            Icon = "@drawable/icon",
            MainLauncher = true,
            LaunchMode = LaunchMode.SingleTask,            
            ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, 
            ScreenOrientation = ScreenOrientation.Portrait)]
    public class AndroidActivity : WaveEngine.Adapter.Application
    {
        private SuperSquid.Game game;

        public AndroidActivity()
        {
			this.FullScreen = true;

			this.DefaultOrientation = WaveEngine.Common.Input.DisplayOrientation.Portrait;
            this.SupportedOrientations = WaveEngine.Common.Input.DisplayOrientation.Portrait;

			// Set the app layout
			this.LayoutId = Resource.Layout.Main;
        }

        public override void Initialize()
        {
            game = new SuperSquid.Game();
            game.Initialize(this);

			this.Window.AddFlags(WindowManagerFlags.KeepScreenOn); 
        }

        public override void Update(TimeSpan elapsedTime)
        {
            game.UpdateFrame(elapsedTime);            
        }

        public override void Draw(TimeSpan elapsedTime)
        {
            game.DrawFrame(elapsedTime);
        }

        public override bool OnKeyDown(Keycode keyCode, KeyEvent e)
        {
            if (keyCode == Keycode.Back && WaveServices.Platform != null)
            {
                WaveServices.Platform.Exit();
            }

            return base.OnKeyDown(keyCode, e);
        }

        protected override void OnPause()
        {
            if (game != null)
            {
                game.OnDeactivated();
            }

            base.OnPause();
        }

        protected override void OnResume()
        {
            base.OnResume();            
            if(game != null)
            {
                game.OnActivated();
            }
        }
    }
}

