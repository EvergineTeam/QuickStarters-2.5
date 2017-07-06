#region Using Statements
using System;
using WaveEngine.Common;
using WaveEngine.Common.Graphics;
using WaveEngine.Framework;
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

			ScreenContext screenContext = new ScreenContext(new P2PScene());	
			WaveServices.ScreenContextManager.To(screenContext);
		}
	}
}
