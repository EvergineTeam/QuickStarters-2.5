using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WaveEngine.Framework.Services;
using WaveEngine.Networking;
using WaveEngine.Networking.Messages;
//using NetTask = System.Threading.Tasks.Task;

namespace MultiplayerTopDownTank.Network
{
    public class Client
    {
        private NetworkService networkService;

        public Client()
        {
            this.networkService = WaveServices.GetService<NetworkService>();
            this.networkService.HostConnected += this.OnHostConnected;
            this.networkService.HostDisconnected += this.OnHostDisconnected;
            this.networkService.MessageReceivedFromHost += this.OnHostMessageReceived;
        }

        private void OnHostConnected(object sender, NetworkEndpoint endpoint)
        {
            throw new NotImplementedException();
        }

        private void OnHostDisconnected(object sender, NetworkEndpoint endpoint)
        {
            throw new NotImplementedException();
        }

        private void OnHostMessageReceived(object sender, NetworkEndpoint fromEndpoint, IncomingMessage receivedMessage)
        {
            throw new NotImplementedException();
        }

        public async Task<List<NetworkEndpoint>> DiscoverHosts(int timeoutmillis)
        {
            List<NetworkEndpoint> hostCollection = new List<NetworkEndpoint>();

            HostDiscovered hostDisceveredHandler = (sender, host) =>
            {
                hostCollection.Add(host);
            };

            this.networkService.HostDiscovered += hostDisceveredHandler;
            this.networkService.DiscoveryHosts(NetworkConfiguration.GameIdentifier, NetworkConfiguration.Port);
            await System.Threading.Tasks.Task.Delay(timeoutmillis);
            this.networkService.HostDiscovered -= hostDisceveredHandler;

            return hostCollection;
        }

        public async System.Threading.Tasks.Task<bool> ConnectToServer(NetworkEndpoint host)
        {
            await this.networkService.ConnectAsync(host);
            return this.networkService.IsClientConnected;
        }

        public void Disconnect()
        {
            if (this.networkService.IsClientConnected)
            {
                this.networkService.Disconnect();
            }
        }
    }
}
