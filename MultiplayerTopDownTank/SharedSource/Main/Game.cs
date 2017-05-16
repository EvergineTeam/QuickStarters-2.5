#region Using Statements
using MultiplayerTopDownTank.Managers;
using WaveEngine.Common;
using WaveEngine.Framework;
using WaveEngine.Framework.Services;
using WaveEngine.Networking;
#endregion

namespace MultiplayerTopDownTank
{
    public class Game : WaveEngine.Framework.Game
    {
        private NavigationManager navigationManager;

        public override void Initialize(IApplication application)
        {
            base.Initialize(application);

            SerializerFactory.DefaultSerializationType = SerializationType.DATACONTRACT;

            this.RegisterServices();

            navigationManager.InitialNavigation();

            WaveServices.ScreenContextManager.SetDiagnosticsActive(true);
        }

        private void RegisterServices()
        {
            navigationManager = new NavigationManager();
            WaveServices.RegisterService(navigationManager);
            WaveServices.RegisterService(new NetworkService());
        }
    }
}