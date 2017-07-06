#region Using Statements
using System;
using P2PNET.TransportLayer.EventArgs;
using WaveEngine.Common;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Components.Cameras;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Components.Graphics3D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Resources;
using WaveEngine.Framework.Services;
using WaveEngine.Framework.UI;
using WaveEngine.Networking.P2P;
#endregion

namespace P2PTank.Scenes
{
    public class P2PScene : Scene
    {
        private Peer2Peer peerManager;

        protected override void CreateScene()
        {
            this.Load(WaveContent.Scenes.MyScene);

            this.peerManager = new Peer2Peer();
            this.peerManager.PeerChange +=  this.OnPeerChanged;
            this.peerManager.MsgReceived += this.OnMsgReceived;

            var button = new WaveEngine.Components.UI.Button()
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Text = "START",
                IsBorder = false,
            };

            button.Click += this.OnStartButtonClicked;
            this.EntityManager.Add(button);
        }

        private async void OnStartButtonClicked(object sender, EventArgs e)
        {
            await this.peerManager.StartBroadcastingAsync();
        }

        private void OnMsgReceived(object sender, MsgReceivedEventArgs e)
        {
        }

        private void OnPeerChanged(object sender, PeerChangeEventArgs e)
        {
        }
    }
}