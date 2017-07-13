using Newtonsoft.Json;
using P2PNET.TransportLayer;
using P2PNET.TransportLayer.EventArgs;
using P2PTank.Entities.P2PMessages;
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

        public P2PManager()
        {
            this.peer2peer = new Peer2Peer();

            this.peer2peer.PeerChange += this.OnPeerChanged;
            this.peer2peer.MsgReceived += this.OnMsgReceived;
        }
        
        protected override void Removed()
        {
            this.peer2peer.PeerChange -= this.OnPeerChanged;
            this.peer2peer.MsgReceived -= this.OnMsgReceived;

            base.Removed();
        }

        public async Task StartAsync()
        {
            await peer2peer.StartAsync();
        }

        public async Task SendMessage(string ipAddress, string message, TransportType transportType)
        {
            await peer2peer.SendMessage(ipAddress, message, transportType);
        }

        public async Task SendBroadcastAsync(string message)
        {
            await peer2peer.SendBroadcastAsync(message);
        }

        public string CreateMessage(P2PMessageType messageType, object content)
        {
            var contentSerialized = JsonConvert.SerializeObject(content);

            return string.Format("{0}/{1}", messageType, contentSerialized);
        }

        public object ReadMessage(string message)
        {
            object messageObject = null;
            var result = message.Split('/');

            P2PMessageType messageType;
            Enum.TryParse(result[0], out messageType);

            switch(messageType)
            {
                case P2PMessageType.CreatePlayer:
                    messageObject = JsonConvert.DeserializeObject<CreatePlayerMessage>(result[1]);
                    break;
            }

            return messageObject;
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