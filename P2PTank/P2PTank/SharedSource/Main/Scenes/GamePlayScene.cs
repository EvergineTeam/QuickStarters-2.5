using System;
using System.Collections.Generic;
using System.Text;
using P2PNET.TransportLayer;
using P2PNET.TransportLayer.EventArgs;
using P2PTank.Behaviors;
using P2PTank.Components;
using P2PTank.Managers;
using WaveEngine.Common.Math;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;

namespace P2PTank.Scenes
{
    public class GamePlayScene : Scene
    {
        private GamePlayManager gamePlayManager;

        protected override void CreateScene()
        {
            this.Load(WaveContent.Scenes.GamePlayScene);

            this.gamePlayManager = this.EntityManager.FindComponentFromEntityPath<GamePlayManager>(GameConstants.ManagerEntityPath);

            var peerManager = new P2PManager();
            peerManager.PeerChange += this.OnPeerChanged;
            peerManager.MsgReceived += this.OnMsgReceived;


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
            //var player = new Entity()
            //    .AddComponent(new Transform2D() { Origin = Vector2.Center })
            //    .AddComponent(new Sprite(WaveContent.Assets.Textures.tankBody_png))
            //    .AddComponent(new SpriteRenderer())
            //    .AddComponent(new TankComponent())
            //    .AddComponent(new PlayerInputBehavior());
            //this.EntityManager.Add(player);
        }

        private void OnMsgReceived(object sender, MsgReceivedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
