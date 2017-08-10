using P2PNET.TransportLayer;
using P2PNET.TransportLayer.EventArgs;
using System;
using System.Text;
using System.Threading.Tasks;

namespace WaveEngine.Networking.P2P
{
    public class Peer2Peer
    {
        private int portNum = 8080;

        private TransportManager transMgr;
        private HeartBeatManager hrtBtMgr;

        public event EventHandler<PeerChangeEventArgs> PeerChange;
        public event EventHandler<MsgReceivedEventArgs> MsgReceived;

        public Peer2Peer()
        {
            this.transMgr = new TransportManager(portNum, true);
            this.hrtBtMgr = new HeartBeatManager("heartbeat", transMgr);

            this.transMgr.PeerChange += OnPeerChange;
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


        private void OnPeerChange(object sender, PeerChangeEventArgs e)
        {
            this.PeerChange?.Invoke(this, e);
        }

        private void OnMsgReceived(object sender, MsgReceivedEventArgs e)
        {
            this.MsgReceived?.Invoke(this, e);
        }
    }
}