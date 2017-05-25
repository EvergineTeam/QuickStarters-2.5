using System.Runtime.Serialization;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Physics2D;
using WaveEngine.Networking;
using WaveEngine.Networking.Messages;

namespace MultiplayerTopDownTank.Components
{
    [DataContract]
    public class BulletNetworkSyncComponent : NetworkSyncComponent
    {
        private Vector2 lastPosition;

        [RequiredComponent]
        protected Transform2D transform;

        [RequiredComponent]
        protected RigidBody2D rigidBody;

        public override bool NeedSendSyncData()
        {
            return
                this.lastPosition != this.transform.Position;
        }

        public override void ReadSyncData(IncomingMessage reader)
        {
            var x = reader.ReadSingle();
            var y = reader.ReadSingle();
            this.transform.Position = new Vector2(x, y);

            this.rigidBody.ResetPosition(new Vector2(x, y));
        }

        public override void WriteSyncData(OutgoingMessage writer)
        {
            this.lastPosition = this.transform.Position;
            writer.Write(this.lastPosition.X);
            writer.Write(this.lastPosition.Y);
        }
    }
}
