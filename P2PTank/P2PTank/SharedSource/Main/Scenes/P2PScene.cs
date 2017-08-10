#region Using Statements
using System;
using P2PNET.TransportLayer.EventArgs;
using WaveEngine.Framework;
using WaveEngine.Framework.UI;
using P2PNET.TransportLayer;
using WaveEngine.Framework.Diagnostic;
using System.Text;
using P2PTank.Managers;
using P2PTank.Entities.P2PMessages;
using System.Linq;
#endregion

namespace P2PTank.Scenes
{
    public class P2PScene : Scene
    {
        private P2PManager peerManager;

        protected override async void CreateScene()
        {
            this.Load(WaveContent.Scenes.MyScene);

            this.peerManager = new P2PManager();
            this.peerManager.PeerChange +=  this.OnPeerChanged;
            this.peerManager.MsgReceived += this.OnMsgReceived;

            var panel = new WaveEngine.Components.UI.StackPanel();
            panel.HorizontalAlignment = HorizontalAlignment.Center;
            panel.VerticalAlignment = VerticalAlignment.Center;
            this.EntityManager.Add(panel);

            var messagesPanel = new WaveEngine.Components.UI.StackPanel
            {
                Orientation = WaveEngine.Components.UI.Orientation.Horizontal
            };

            panel.Add(messagesPanel);

            var createPlayerButon = new WaveEngine.Components.UI.Button()
            {
                Text = "Create Player",
                Width = 200,
                IsBorder = false
            };
            messagesPanel.Add(createPlayerButon);
            createPlayerButon.Click += OnCreatePlayerButonClick;

            var moveButon = new WaveEngine.Components.UI.Button()
            {
                Text = "Move",
                IsBorder = false
            };
            messagesPanel.Add(moveButon);
            moveButon.Click += OnMoveButonClick;

            var shootButon = new WaveEngine.Components.UI.Button()
            {
                Text = "Shoot",
                IsBorder = false
            };
            messagesPanel.Add(shootButon);
            shootButon.Click += OnShootButonClick;

            var dieButon = new WaveEngine.Components.UI.Button()
            {
                Text = "Die",
                IsBorder = false
            };
            messagesPanel.Add(dieButon);
            dieButon.Click += OnDieButonClick;

            await peerManager.StartAsync();
        }

        private async void OnCreatePlayerButonClick(object sender, EventArgs e)
        {
            var createPlayerMessage = new CreatePlayerMessage
            {
                IpAddress = "192.168.1.1",
                PlayerId = "1"
            };

            var message = peerManager.CreateMessage(P2PMessageType.CreatePlayer, createPlayerMessage);

            await peerManager.SendBroadcastAsync(message);
        }

        private async void OnMoveButonClick(object sender, EventArgs e)
        {
            Random rnd = new Random();
            var moveMessage = new MoveMessage
            {
                PlayerId = "1",
                X = rnd.Next(1, 300),
                Y = rnd.Next(1, 300)
            };

            var message = peerManager.CreateMessage(P2PMessageType.Move, moveMessage);

            await peerManager.SendBroadcastAsync(message);
        }

        private async void OnShootButonClick(object sender, EventArgs e)
        {
            Random rnd = new Random();
            var shootMessage = new ShootMessage
            {
                PlayerId = "1",
                X = rnd.Next(1, 300),
                Y = rnd.Next(1, 300)
            };

            var message = peerManager.CreateMessage(P2PMessageType.Shoot, shootMessage);

            await peerManager.SendBroadcastAsync(message);
        }

        private async void OnDieButonClick(object sender, EventArgs e)
        {
            var destroyMessage = new DestroyPlayerMessage
            {
                PlayerId = "1"
            };

            var message = peerManager.CreateMessage(P2PMessageType.Destroy, destroyMessage);

            await peerManager.SendBroadcastAsync(message);
        }

        private void OnMsgReceived(object sender, MsgReceivedEventArgs e)
        {
            var messageReceived = Encoding.ASCII.GetString(e.Message);

            Labels.Add("OnMsgReceived", messageReceived);

            var result = peerManager.ReadMessage(messageReceived);

            if(result.Any())
            {
                var message = result.FirstOrDefault();

                if(message.Value != null)
                {
                    switch(message.Key)
                    {
                        case P2PMessageType.CreatePlayer:
                            var createPlayerData = message.Value as CreatePlayerMessage;
                            break;
                        case P2PMessageType.Move:
                            var moveData = message.Value as MoveMessage;
                            break;
                        case P2PMessageType.Rotate:
                            break;
                        case P2PMessageType.Shoot:
                            var shootData = message.Value as ShootMessage;
                            break;
                        case P2PMessageType.Destroy:
                            var destroyData = message.Value as DestroyPlayerMessage;
                            break;
                    }
                }
            }
        }

        private void OnPeerChanged(object sender, PeerChangeEventArgs e)
        {
            foreach (Peer peer in e.Peers)
            {
                Labels.Add("OnPeerChanged", peer.IpAddress);
            }
        }
    }
}