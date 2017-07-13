#region Using Statements
using WaveEngine.Common;
using WaveEngine.Framework.Services;
using P2PTank.Scenes;
#endregion

namespace P2PTank
{
    public class Game : WaveEngine.Framework.Game
	{
		public override void Initialize(IApplication application)
		{
			base.Initialize(application);

			//ScreenContext screenContext = new ScreenContext(new P2PScene());
            ScreenContext screenContext = new ScreenContext(new GamePlayScene());

            WaveServices.ScreenContextManager.To(screenContext);
            WaveServices.ScreenContextManager.SetDiagnosticsActive(true);
		}
	}
}
