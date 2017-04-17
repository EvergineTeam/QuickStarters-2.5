#region Using Statements
using MultiplayerTopDownTank.Managers;
using WaveEngine.Common;
using WaveEngine.Framework.Services;
#endregion

namespace MultiplayerTopDownTank
{
    public class Game : WaveEngine.Framework.Game
    {
        public override void Initialize(IApplication application)
        {
            base.Initialize(application);

            var navigationManager = new NavigationManager();
            WaveServices.RegisterService(navigationManager);

            navigationManager.NavigateToGame();
        }
    }
}