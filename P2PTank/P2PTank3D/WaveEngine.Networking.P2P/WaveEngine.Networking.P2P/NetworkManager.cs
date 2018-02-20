using System;
using System.Text;
using System.Threading.Tasks;
using Networking.P2P.TransportLayer;
using Networking.P2P.TransportLayer.EventArgs;
using Sockets.Plugin.Abstractions;
using Sockets.Plugin;
using System.Linq;

namespace Networking.P2P
{
    public class NetworkManager
    {
        private int portNum;
        private string interfaceName;

        private TransportManager transMgr;
        // private HeartBeatManager hrtBtMgr;

        public event EventHandler<PeerPlayerChangeEventArgs> PeerPlayerChange;
        public event EventHandler<MsgReceivedEventArgs> MsgReceived;

        public string IpAddress
        {
            get
            {
                return this.transMgr?.SelectedCommsInterface?.IpAddress;
            }
        }

        public NetworkManager(int port = 8080, string interfaceName = "")
        {
            this.portNum = port;
            this.interfaceName = interfaceName;
        }

        public async Task StartAsync()
        {
            ICommsInterface commsInterface = null;

            if (!string.IsNullOrWhiteSpace(this.interfaceName))
            {
                var interfaces = await CommsInterface.GetAllInterfacesAsync();
                commsInterface = interfaces.FirstOrDefault(i => i.Name.ToLower().Equals(this.interfaceName.ToLower()));
            }

            this.transMgr = new TransportManager(commsInterface, this.portNum, false, false);
            this.transMgr.PeerPlayerChange += this.OnPeerChange;
            this.transMgr.MsgReceived += this.OnMsgReceived;

            await this.transMgr.StartAsync();
        }

        //public async Task StartBroadcastingAsync()
        //{
        //    this.hrtBtMgr = new HeartBeatManager("heartbeat", transMgr);
        //    this.hrtBtMgr.StartBroadcasting();
        //    await this.transMgr.StartAsync();
        //}

        public async Task SendMessage(string message)
        {
            byte[] messageBits = Encoding.UTF8.GetBytes(message);
            await this.transMgr.SendBroadcastAsyncUDP(messageBits);
        }

        public async Task SendMessage(string ipAddress, string message, TransportType transportType)
        {
            byte[] msgBits = Encoding.UTF8.GetBytes(message);

            if (transportType == TransportType.UDP)
            {
                await this.transMgr.SendAsyncUDP(ipAddress, msgBits);
            }
            else
            {
                await this.transMgr.SendAsyncTCP(ipAddress, msgBits);
            }
        }

        public async Task SendBroadcastAsync(string message)
        {
            byte[] msgBits = Encoding.UTF8.GetBytes(message);

            await transMgr.SendBroadcastAsyncUDP(msgBits);
        }

        private void OnPeerChange(object sender, PeerPlayerChangeEventArgs e)
        {
            this.PeerPlayerChange?.Invoke(this, e);
        }

        private void OnMsgReceived(object sender, MsgReceivedEventArgs e)
        {
            this.MsgReceived?.Invoke(this, e);
        }
    }
}