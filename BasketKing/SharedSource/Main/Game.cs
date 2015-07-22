#region Using Statements
using BasketKing.Managers;
using System;
using WaveEngine.Common;
using WaveEngine.Common.Graphics;
using WaveEngine.Components;
using WaveEngine.Framework;
using WaveEngine.Framework.Services;
#endregion

namespace BasketKing
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

            ViewportManager vm = WaveServices.ViewportManager;
            vm.Activate(1024, 768, ViewportManager.StretchMode.UniformToFill);

            ScreenContext screenContext = new ScreenContext(new GamePlayScene()) { Behavior = ScreenContextBehaviors.DrawInBackground };	
			WaveServices.ScreenContextManager.To(screenContext);
        }
    }
}
