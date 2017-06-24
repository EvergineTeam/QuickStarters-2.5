using System;
using WaveEngine.Common;
using WaveEngine.Framework.Services;
using WaveEngine.Networking;
using WaveEngine.Networking.Messages;

namespace MultiplayerTopDownTank.Network
{
    public class Server : Service
    {
        private NetworkService networkService;

        public Server()
        {
            this.networkService = WaveServices.GetService<NetworkService>();
        }

        public void StartServer()
        {
            this.networkService.ClientConnected += this.NetworkService_ClientConnected;
            this.networkService.ClientDisconnected += this.NetworkService_ClientDisconnected;
            this.networkService.MessageReceivedFromClient += this.ClientMessageReceived;

            this.networkService.InitializeHost(NetworkConfiguration.GameIdentifier, NetworkConfiguration.Port);
        }

        public void DestroyServer()
        {
            this.networkService.Disconnect();
        }

        private void NetworkService_ClientConnected(object sender, NetworkEndpoint endpoint)
        {
            throw new NotImplementedException();
        }

        private void NetworkService_ClientDisconnected(object sender, NetworkEndpoint endpoint)
        {
            throw new NotImplementedException();
        }

        private void ClientMessageReceived(object sender, NetworkEndpoint fromEndpoint, IncomingMessage receivedMessage)
        {
            throw new NotImplementedException();
        }
    }
}
