using Newtonsoft.Json;
using P2PTank.Entities.P2PMessages;
using System;
using System.Threading.Tasks;
using WaveEngine.Framework;
using System.Collections.Generic;
using System.Diagnostics;
using P2PTank.Managers.P2PMessages;
using Networking.P2P.TransportLayer.EventArgs;
using Networking.P2P;
using Networking.P2P.TransportLayer;

namespace P2PTank.Managers
{
    public class P2PManager : Component
    {
        private NetworkManager peer2peer;

        public event EventHandler<PeerPlayerChangeEventArgs> PeerPlayerChange;
        public event EventHandler<MsgReceivedEventArgs> MsgReceived;

        public string IpAddress
        {
            get
            {
                return this.peer2peer.IpAddress;
            }
        }

        public P2PManager(int port = 8080, string interfaceName = null)
        {
            this.peer2peer = new NetworkManager(port, interfaceName);
            this.peer2peer.PeerPlayerChange += this.OnPeerChanged;
            this.peer2peer.MsgReceived += this.OnMsgReceived;
        }

        protected override void Removed()
        {
            this.peer2peer.PeerPlayerChange -= this.OnPeerChanged;
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

        public Dictionary<P2PMessageType, object> ReadMessage(string message)
        {
            Dictionary<P2PMessageType, object> messageObject = new Dictionary<P2PMessageType, object>();
            var result = message.Split('/');

            P2PMessageType messageType;
            Enum.TryParse(result[0], out messageType);

            try
            {
                switch (messageType)
                {
                    case P2PMessageType.CreatePlayer:
                        messageObject.Add(
                            P2PMessageType.CreatePlayer,
                            JsonConvert.DeserializeObject<CreatePlayerMessage>(result[1]));
                        break;
                    case P2PMessageType.Move:
                        messageObject.Add(
                            P2PMessageType.Move,
                            JsonConvert.DeserializeObject<MoveMessage>(result[1]));
                        break;
                    case P2PMessageType.Rotate:
                        messageObject.Add(
                            P2PMessageType.Rotate,
                            JsonConvert.DeserializeObject<RotateMessage>(result[1]));
                        break;
                    case P2PMessageType.Shoot:
                        messageObject.Add(
                            P2PMessageType.Shoot,
                            JsonConvert.DeserializeObject<ShootMessage>(result[1]));
                        break;
                    case P2PMessageType.HitPlayer:
                        messageObject.Add(
                            P2PMessageType.HitPlayer,
                            JsonConvert.DeserializeObject<HitPlayerMessage>(result[1]));
                        break;
                    case P2PMessageType.DestroyPlayer:
                        messageObject.Add(
                            P2PMessageType.DestroyPlayer,
                            JsonConvert.DeserializeObject<DestroyPlayerMessage>(result[1]));
                        break;
                    case P2PMessageType.BulletCreate:
                        messageObject.Add(
                            P2PMessageType.BulletCreate,
                            JsonConvert.DeserializeObject<BulletCreateMessage>(result[1]));
                        break;
                    case P2PMessageType.BulletMove:
                        messageObject.Add(
                            P2PMessageType.BulletMove,
                            JsonConvert.DeserializeObject<BulletMoveMessage>(result[1]));
                        break;
                    case P2PMessageType.BulletDestroy:
                        messageObject.Add(
                            P2PMessageType.BulletDestroy,
                            JsonConvert.DeserializeObject<BulletDestroyMessage>(result[1]));
                        break;
                    case P2PMessageType.CreatePowerUp:
                        messageObject.Add(
                            P2PMessageType.CreatePowerUp,
                            JsonConvert.DeserializeObject<CreatePowerUpMessage>(result[1]));
                        break;
                    case P2PMessageType.DestroyPowerUp:
                        messageObject.Add(
                            P2PMessageType.DestroyPowerUp,
                            JsonConvert.DeserializeObject<DestroyPowerUpMessage>(result[1]));
                        break;
                    case P2PMessageType.RemovePowerUp:
                        messageObject.Add(
                            P2PMessageType.RemovePowerUp,
                            JsonConvert.DeserializeObject<RemovePowerUpMessage>(result[1]));
                        break;
                    case P2PMessageType.PlayerRequest:
                        messageObject.Add(
                            P2PMessageType.PlayerRequest,
                            JsonConvert.DeserializeObject<PlayerRequestMessage>(result[1]));
                        break;
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return messageObject;
        }

        private void OnMsgReceived(object sender, MsgReceivedEventArgs e)
        {
            this.MsgReceived?.Invoke(this, e);
        }

        private void OnPeerChanged(object sender, PeerPlayerChangeEventArgs e)
        {
            this.PeerPlayerChange?.Invoke(this, e);
        }
    }
}