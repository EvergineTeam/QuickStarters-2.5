#region Using Statements
using SurvivorNinja.Managers;
using System;
using WaveEngine.Common;
using WaveEngine.Common.Graphics;
using WaveEngine.Components;
using WaveEngine.Framework;
using WaveEngine.Framework.Services;
#endregion

namespace SurvivorNinja
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

            ViewportManager vm = WaveServices.ViewportManager;
            vm.Activate(1024, 768, ViewportManager.StretchMode.Uniform);

			ScreenContext screenContext = new ScreenContext(new GamePlayScene());	
			WaveServices.ScreenContextManager.To(screenContext);
        }
    }
}
