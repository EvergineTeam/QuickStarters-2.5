#region Using Statements
using System;
using P2PNET.TransportLayer.EventArgs;
using WaveEngine.Framework;
using WaveEngine.Framework.UI;
using WaveEngine.Networking.P2P;
using P2PNET.TransportLayer;
using WaveEngine.Framework.Diagnostic;
using System.Text;
#endregion

namespace P2PTank.Scenes
{
    public class P2PScene : Scene
    {
        private Peer2Peer peerManager;

        private WaveEngine.Components.UI.TextBox _textBox1;
        private WaveEngine.Components.UI.TextBox _textBox2;
        private WaveEngine.Components.UI.TextBox _textBox3;
        private WaveEngine.Components.UI.TextBox _textBox4;

        protected override async void CreateScene()
        {
            this.Load(WaveContent.Scenes.MyScene);

            this.peerManager = new Peer2Peer();
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

            var button = new WaveEngine.Components.UI.Button()
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Text = "START",
                IsBorder = false
            };

            button.Click += this.OnStartButtonClicked;
            panel.Add(button);

            await peerManager.StartAsync();
        }

        private async void OnStartButtonClicked(object sender, EventArgs e)
        {
            var ipAddress = string.Format("{0}.{1}.{2}.{3}", _textBox1.Text, _textBox2.Text, _textBox3.Text, _textBox4.Text);

            await peerManager.SendMessage(ipAddress, "Test", TransportType.TCP);
            
        }

        private void OnMsgReceived(object sender, MsgReceivedEventArgs e)
        {
            var message = Encoding.ASCII.GetString(e.Message);

            Labels.Add("OnMsgReceived", message);
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