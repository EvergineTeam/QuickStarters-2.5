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
using Newtonsoft.Json;
#endregion

namespace P2PTank.Scenes
{
    public class P2PScene : Scene
    {
        private P2PManager peerManager;

        private WaveEngine.Components.UI.TextBox _textBox1;
        private WaveEngine.Components.UI.TextBox _textBox2;
        private WaveEngine.Components.UI.TextBox _textBox3;
        private WaveEngine.Components.UI.TextBox _textBox4;

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

            var label = new WaveEngine.Components.UI.TextBlock
            {
                Text = "Insert your Opponent IP Address"
            };

            panel.Add(label);

            var ipPanel = new WaveEngine.Components.UI.StackPanel
            {
                Orientation = WaveEngine.Components.UI.Orientation.Horizontal
            };

            panel.Add(ipPanel);

            _textBox1 = new WaveEngine.Components.UI.TextBox
            {
                Text = string.Empty,
                Width = 50,
                Margin = new Thickness(6)
            };

            ipPanel.Add(_textBox1);

            _textBox2 = new WaveEngine.Components.UI.TextBox
            {
                Text = string.Empty,
                Width = 50,
                Margin = new Thickness(6)
            };

            ipPanel.Add(_textBox2);

            _textBox3 = new WaveEngine.Components.UI.TextBox
            {
                Text = string.Empty,
                Width = 50,
                Margin = new Thickness(6)
            };

            ipPanel.Add(_textBox3);


            _textBox4 = new WaveEngine.Components.UI.TextBox
            {
                Text = string.Empty,
                Width = 50,
                Margin = new Thickness(6)
            };

            ipPanel.Add(_textBox4);

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
                IpAddress = "10.4.1.1",
                PlayerId = "1"
            };

            var ipAddress = string.Format("{0}.{1}.{2}.{3}", _textBox1.Text, _textBox2.Text, _textBox3.Text, _textBox4.Text);

            await peerManager.SendMessage(
                ipAddress, 
                peerManager.CreateMessage(P2PMessageType.CreatePlayer, createPlayerMessage), 
                TransportType.TCP);
        }

        private async void OnMoveButonClick(object sender, EventArgs e)
        {
            var moveMessage = new MoveMessage
            {
                PlayerId = "1",
                X = 300,
                Y = 350
            };

            var moveMessageSerialized = JsonConvert.SerializeObject(moveMessage);

            var ipAddress = string.Format("{0}.{1}.{2}.{3}", _textBox1.Text, _textBox2.Text, _textBox3.Text, _textBox4.Text);

            await peerManager.SendMessage(ipAddress, moveMessageSerialized, TransportType.TCP);
        }

        private async void OnShootButonClick(object sender, EventArgs e)
        {
            var shootMessage = new ShootMessage
            {
                PlayerId = "1",
                X = 30,
                Y = 35
            };

            var shootMessageSerialized = JsonConvert.SerializeObject(shootMessage);

            var ipAddress = string.Format("{0}.{1}.{2}.{3}", _textBox1.Text, _textBox2.Text, _textBox3.Text, _textBox4.Text);

            await peerManager.SendMessage(ipAddress, shootMessageSerialized, TransportType.TCP);
        }

        private async void OnDieButonClick(object sender, EventArgs e)
        {
            var destroyMessage = new DestroyMessage
            {
                PlayerId = "1"
            };

            var destroyMessageSerialized = JsonConvert.SerializeObject(destroyMessage);

            var ipAddress = string.Format("{0}.{1}.{2}.{3}", _textBox1.Text, _textBox2.Text, _textBox3.Text, _textBox4.Text);

            await peerManager.SendMessage(ipAddress, destroyMessageSerialized, TransportType.TCP);
        }

        private void OnMsgReceived(object sender, MsgReceivedEventArgs e)
        {
            var message = Encoding.ASCII.GetString(e.Message);

            Labels.Add("OnMsgReceived", message);

            var result = peerManager.ReadMessage(message);
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