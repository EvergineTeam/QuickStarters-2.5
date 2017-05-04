using System;
using System.Diagnostics;
using WaveEngine.Framework;
using WaveEngine.Framework.Services;
using WaveEngine.Framework.UI;
using WaveEngine.Networking;
using WaveEngine.Framework.Animation;
using MultiplayerTopDownTank.Managers;

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

        private void OnStartButtonClicked(object sender, EventArgs args)
        {
            DiscoverHosts();
        }

        private void DiscoverHosts()
        {
            try
            {
                this.networkService.InitializeHost(NetworkConfiguration.GameIdentifier, NetworkConfiguration.Port);
                this.DiscoverHosts();
            }
            catch
            {
                this.DisableHostDiscoveryAndCleanButtons();
                this.networkService.HostDiscovered += this.OnHostDiscovered;
                this.networkService.DiscoveryHosts(NetworkConfiguration.GameIdentifier, NetworkConfiguration.Port);
            }
        }

        private void DisableHostDiscoveryAndCleanButtons()
        {
            this.networkService.HostDiscovered -= this.OnHostDiscovered;
        }

        private void OnHostDiscovered(object sender, Host host)
        {
            if (host != null)
            {
                ConnectToHost(host);
            }
        }

        private async void ConnectToHost(Host host)
        {
            try
            {
                await this.networkService.ConnectAsync(host);
                this.navigationManager.NavigateToLobby();
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
            }
        }
    }
}