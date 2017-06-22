using System;
using WaveEngine.Framework;
using WaveEngine.Framework.Services;
using WaveEngine.Framework.UI;
using WaveEngine.Networking;
using WaveEngine.Framework.Animation;
using MultiplayerTopDownTank.Managers;
using System.Threading.Tasks;

namespace MultiplayerTopDownTank.Scenes
{
    public class MenuScene : Scene
    {
        private readonly NetworkService networkService;
        private readonly NavigationManager navigationManager;

        public MenuScene()
        {
            this.networkService = WaveServices.GetService<NetworkService>();
            this.navigationManager = WaveServices.GetService<NavigationManager>();
        }

        protected override void CreateScene()
        {
            this.Load(WaveContent.Scenes.MenuScene);
            this.CreateUI();
        }

        private void CreateUI()
        {
            var button = new WaveEngine.Components.UI.Button()
            {
                Margin = new Thickness(0, 0, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Text = "PLAY",
                IsBorder = false,
                BackgroundImage = WaveContent.Assets.Gui.Buttons.BTN_GRAY_RECT_OUT_png,
                PressedBackgroundImage = WaveContent.Assets.Gui.Buttons.BTN_GRAY_RECT_IN_png
            };

            button.Entity.AddComponent(new AnimationUI());

            button.Click += this.OnStartButtonClicked;

            this.EntityManager.Add(button);
        }

        private async void OnStartButtonClicked(object sender, EventArgs args)
        {
            await SearchOrCreateHost();
        }

        private async System.Threading.Tasks.Task SearchOrCreateHost()
        {
            var discoveredHost = await this.WaitForDiscoverHostAsync(TimeSpan.FromSeconds(3));

            //NetworkEndpoint discoveredHost = new NetworkEndpoint
            //{
            //    Address = "10.4.1.42",
            //    Port = NetworkConfiguration.Port,
            //};

            if (discoveredHost == null)
            {
                this.networkService.InitializeHost(NetworkConfiguration.GameIdentifier, NetworkConfiguration.Port);
                await this.SearchOrCreateHost();
            }
            else
            {
                this.networkService.Connect(NetworkConfiguration.GameIdentifier, discoveredHost);
                this.navigationManager.NavigateToLobby();
            }
        }

        private async Task<NetworkEndpoint> WaitForDiscoverHostAsync(TimeSpan timeOut)
        {
            NetworkEndpoint discoveredHost = null;
            HostDiscovered hostDisceveredHandler = (sender, host) =>
            {
                discoveredHost = host;
            };

            this.networkService.HostDiscovered += hostDisceveredHandler;
            this.networkService.DiscoveryHosts(NetworkConfiguration.GameIdentifier, NetworkConfiguration.Port);
            await System.Threading.Tasks.Task.Delay(timeOut);
            this.networkService.HostDiscovered -= hostDisceveredHandler;

            return discoveredHost;
        }
    }
}