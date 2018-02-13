using System;
using System.Text;
using WaveEngine.Framework;
using P2PTank.Entities.P2PMessages;
using P2PTank.Managers;
using System.Linq;
using WaveEngine.Framework.Graphics;
using Networking.P2P.TransportLayer.EventArgs;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Diagnostic;

namespace P2PTank.Behaviors
{
    public class NetworkInputBehavior : Behavior
    {
        private P2PManager peerManager;

        private Vector2 desiredPosition;

        [RequiredComponent]
        private Transform2D transform = null;

        private Transform2D barrelTransform = null;

        public string PlayerID { get; set; }

        public NetworkInputBehavior(P2PManager p2pManager)
        {
            this.peerManager = p2pManager;

            this.peerManager.MsgReceived += this.OnMessageReceived;
        }

        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            var barrelEntity = this.Owner.FindChild(GameConstants.EntitynameTankBarrel);
            this.barrelTransform = barrelEntity.FindComponent<Transform2D>();
        }

        private void OnMessageReceived(object sender, MsgReceivedEventArgs e)
        {
            var messageReceived = Encoding.ASCII.GetString(e.Message);

            Labels.Add("OnMsgReceived NetworkInputBehavior", messageReceived);

            var result = this.peerManager.ReadMessage(messageReceived);

            if (result.Any())
            {
                var message = result.FirstOrDefault();

                if (message.Value != null)
                {
                    switch (message.Key)
                    {
                        case P2PMessageType.Move:
                            var moveData = message.Value as MoveMessage;

                            if (this.PlayerID.Equals(moveData.PlayerId))
                            {
                                this.Move(moveData.X, moveData.Y);
                            }
                            break;
                        case P2PMessageType.Rotate:
                            var rotateData = message.Value as RotateMessage;

                            if (this.PlayerID.Equals(rotateData.PlayerId))
                            {
                                this.Rotate(rotateData.Rotation);
                            }
                            break;
                    }
                }
            }
        }

        protected override void Update(TimeSpan gameTime)
        {
            if (this.transform.Position != this.desiredPosition)
            {
                this.transform.Position = Vector2.Lerp(this.transform.Position, this.desiredPosition, 0.2f);
            }
        }

        private void Move(float x, float y)
        {
            if (transform == null)
                return;

            var pos = this.transform.Position;
            pos.X = x;
            pos.Y = y;

            this.desiredPosition = pos;
            // this.transform.Position = pos;
        }

        private void Rotate(float angle)
        {
            if (transform == null)
                return;

            this.transform.Rotation = angle;
        }

        private void BarrelRotate(float angle)
        {
            if (barrelTransform == null)
                return;

            this.barrelTransform.Rotation = angle;
        }

        protected override void Removed()
        {
            base.Removed();

            this.peerManager.MsgReceived -= this.OnMessageReceived;
        }
    }
}
