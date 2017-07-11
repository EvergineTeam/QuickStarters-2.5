using P2PNET.TransportLayer;
using P2PNET.TransportLayer.EventArgs;
using System;
using System.Threading.Tasks;
using WaveEngine.Framework;
using WaveEngine.Networking.P2P;

namespace P2PTank.Managers
{
    public class P2PManager : Component
    {
        private Peer2Peer peer2peer;

        public event EventHandler<PeerChangeEventArgs> PeerChange;
        public event EventHandler<MsgReceivedEventArgs> MsgReceived;

        protected override async void Initialize()
        {
            this.peer2peer = new Peer2Peer();

            this.peer2peer.PeerChange += this.OnPeerChanged;
            this.peer2peer.MsgReceived += this.OnMsgReceived;

            await peer2peer.StartAsync();

            base.Initialize();
        }

        protected override void Removed()
        {
            this.peer2peer.PeerChange -= this.OnPeerChanged;
            this.peer2peer.MsgReceived -= this.OnMsgReceived;

            base.Removed();
        }

        public async Task SendMessage(string ipAddress, string message, TransportType transportType)
        {
            await peer2peer.SendMessage(ipAddress, message, transportType);
        }

        private void OnMsgReceived(object sender, MsgReceivedEventArgs e)
        {
            this.MsgReceived?.Invoke(this, e);
        }

        private void OnPeerChanged(object sender, PeerChangeEventArgs e)
        {
            this.PeerChange?.Invoke(this, e);
        }
    }
}