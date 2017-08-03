using System;
using System.Collections.Generic;
using System.Text;
using P2PNET.TransportLayer;
using P2PNET.TransportLayer.EventArgs;
using P2PTank.Behaviors;
using P2PTank.Managers;
using P2PTank.Components;
using WaveEngine.Common.Math;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;

namespace P2PTank.Scenes
{
    public class GamePlayScene : Scene
    {
        private GamePlayManager gamePlayManager;

        private P2PManager peerManager;

        protected async override void CreateScene()
        {
            this.Load(WaveContent.Scenes.GamePlayScene);

            this.gamePlayManager = this.EntityManager.FindComponentFromEntityPath<GamePlayManager>(GameConstants.ManagerEntityPath);

            this.peerManager = new P2PManager();
            this.peerManager.PeerChange += this.OnPeerChanged;
            this.peerManager.MsgReceived += this.OnMsgReceived;

            await this.peerManager.StartAsync();

            //var player = new Entity()
            //    .AddComponent(new Transform2D() { Origin = Vector2.Center })
            //    .AddComponent(new Sprite(WaveContent.Assets.Textures.tankBody_png))
            //    .AddComponent(new SpriteRenderer())
            //    .AddComponent(new TankComponent())
            //    .AddComponent(new PlayerInputBehavior());
            //this.EntityManager.Add(player);
        }

        private List<Peer> peers = new List<Peer>();

        private void OnPeerChanged(object sender, PeerChangeEventArgs e)
        {
            foreach(var peer in e.Peers)
            {
                if(!peers.Contains(peer))
                {
                    this.CreateFoe();
                }
            }
        }

        private void CreateFoe()
        {
            var foe = new Entity()
                .AddComponent(new Transform2D() { Origin = Vector2.Center })
                .AddComponent(new Sprite(WaveContent.Assets.Textures.tankBody_png))
                .AddComponent(new SpriteRenderer())
                .AddComponent(new TankComponent())
                .AddComponent(new NetworkInputBehavior(this.peerManager));
            this.EntityManager.Add(foe);
        }

        private void CreatePlayer()
        {
            var foe = new Entity()
                .AddComponent(new Transform2D() { Origin = Vector2.Center })
                .AddComponent(new Sprite(WaveContent.Assets.Textures.tankBody_png))
                .AddComponent(new SpriteRenderer())
                .AddComponent(new TankComponent())
                .AddComponent(new PlayerInputBehavior(this.peerManager, "player1"));
            this.EntityManager.Add(foe);
        }

        private void OnMsgReceived(object sender, MsgReceivedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
