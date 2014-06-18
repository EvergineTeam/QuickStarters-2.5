using System;
using System.Collections.Generic;
using WaveEngine.Framework.Services;
using System;
using WaveEngine.Adapter;
using WaveEngine.Common.Input;
using WaveEngine.Framework.Services;

namespace LauncherWindowsStore
{
	public class App : Application
    {
        private SuperSquidProject.Game game;

        public override void Draw(TimeSpan elapsedTime)
        {
            game.DrawFrame(elapsedTime);
        }

        public override void Initialize()
        {
            game = new SuperSquidProject.Game();
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

