using System;
using System.Collections.Generic;
using WaveEngine.Framework.Services;
using WaveEngine.Adapter;
using WaveEngine.Common.Input;

namespace LauncherWindowsStore
{
	public class App : Application
    {
        private FlyingKiteProject.Game game;

        public override void Draw(TimeSpan elapsedTime)
        {
            game.DrawFrame(elapsedTime);
        }

        public override void Initialize()
        {
            game = new FlyingKiteProject.Game();
            game.Initialize(this);
        }

        public override void Update(TimeSpan elapsedTime)
        {
            if (!game.HasExited)
            {
                if (WaveServices.Input.KeyboardState.Escape == ButtonState.Pressed)
                {
                    WaveServices.Platform.Exit();
                }
                else
                {
                    game.UpdateFrame(elapsedTime);
                }
            }
        }
        
        public override void OnResuming()
        {
            game.OnActivated();
        }

        public override void OnSuspending()
        {
            game.OnDeactivated();
        }
    }
}

