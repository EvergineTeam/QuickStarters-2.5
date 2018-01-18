using System;
using System.Text;
using System.Threading.Tasks;
using Networking.P2P.TransportLayer;
using Networking.P2P.TransportLayer.EventArgs;

namespace Networking.P2P
{
    public class NetworkManager
    {
        private int portNum = 8080;

        private TransportManager transMgr;
        private HeartBeatManager hrtBtMgr;

        public event EventHandler<PeerPlayerChangeEventArgs> PeerPlayerChange;
        public event EventHandler<MsgReceivedEventArgs> MsgReceived;

        public NetworkManager()
        {
            this.transMgr = new TransportManager(portNum, true);
            this.hrtBtMgr = new HeartBeatManager("heartbeat", transMgr);

            this.transMgr.PeerPlayerChange += OnPeerChange;
            this.transMgr.MsgReceived += OnMsgReceived;
        }

        public async Task StartAsync()
        {
            await this.transMgr.StartAsync();
        }

        public async Task StartBroadcastingAsync()
        {
            this.hrtBtMgr.StartBroadcasting();
            await this.transMgr.StartAsync();
        }

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

        public async Task<string> GetIpAddress()
        {
            return await this.transMgr.GetIpAddress();
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