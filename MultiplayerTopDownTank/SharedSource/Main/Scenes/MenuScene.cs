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
        private readonly NavigationManager navigationManager;

        public MenuScene()
        {
            this.navigationManager = WaveServices.GetService<NavigationManager>();
        }

        protected override void CreateScene()
        {
            this.Load(WaveContent.Scenes.MenuScene);
            this.CreateUI();
        }

        private void CreateUI()
        {
            var joinButton = this.CreateButton(HorizontalAlignment.Center, VerticalAlignment.Top, "Join a match");
            joinButton.Click += this.OnJoinMatchButtonClicked;
            this.EntityManager.Add(joinButton);

            var createServerButton = this.CreateButton(HorizontalAlignment.Center, VerticalAlignment.Bottom, "Create Server");
            createServerButton.Click += this.OnCreateServerButtonClicked;
            this.EntityManager.Add(createServerButton);
        }

        private void OnCreateServerButtonClicked(object sender, EventArgs e)
        {
            this.navigationManager.NavigateToLobby(true);
        }

        private void OnJoinMatchButtonClicked(object sender, EventArgs args)
        {
            this.navigationManager.NavigateToLobby(false);
            //await SearchOrCreateHost();
        }

        private WaveEngine.Components.UI.Button CreateButton(HorizontalAlignment horizontal, VerticalAlignment vertical, string text)
        {
            var button = new WaveEngine.Components.UI.Button()
            {
                Margin = new Thickness(0, 0, 0, 0),
                HorizontalAlignment = horizontal,
                VerticalAlignment = vertical,
                Text = text,
                IsBorder = false,
                BackgroundImage = WaveContent.Assets.Gui.Buttons.BTN_GRAY_RECT_OUT_png,
                PressedBackgroundImage = WaveContent.Assets.Gui.Buttons.BTN_GRAY_RECT_IN_png
            };
            button.Entity.AddComponent(new AnimationUI());

            return button;
        }

       

        //private async System.Threading.Tasks.Task SearchOrCreateHost()
        //{
        //    var discoveredHost = await this.WaitForDiscoverHostAsync(TimeSpan.FromSeconds(3));

        //    //NetworkEndpoint discoveredHost = new NetworkEndpoint
        //    //{
        //    //    Address = "10.4.1.42",
        //    //    Port = NetworkConfiguration.Port,
        //    //};

        //    if (discoveredHost == null)
        //    {
        //        this.networkService.InitializeHost(NetworkConfiguration.GameIdentifier, NetworkConfiguration.Port);
        //        await this.SearchOrCreateHost();
        //    }
        //    else
        //    {
        //        this.networkService.Connect(NetworkConfiguration.GameIdentifier, discoveredHost);
        //        this.navigationManager.NavigateToLobby();
        //    }
        //}

        //private async Task<NetworkEndpoint> WaitForDiscoverHostAsync(TimeSpan timeOut)
        //{
        //    NetworkEndpoint discoveredHost = null;
        //    HostDiscovered hostDisceveredHandler = (sender, host) =>
        //    {
        //        discoveredHost = host;
        //    };

        //    this.networkService.HostDiscovered += hostDisceveredHandler;
        //    this.networkService.DiscoveryHosts(NetworkConfiguration.GameIdentifier, NetworkConfiguration.Port);
        //    await System.Threading.Tasks.Task.Delay(timeOut);
        //    this.networkService.HostDiscovered -= hostDisceveredHandler;

        //    return discoveredHost;
        //}
    }
}