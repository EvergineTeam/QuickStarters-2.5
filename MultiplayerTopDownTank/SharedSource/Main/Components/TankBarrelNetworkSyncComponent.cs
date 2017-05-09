using System.Runtime.Serialization;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Networking;
using WaveEngine.Networking.Messages;

namespace MultiplayerTopDownTank.Components
{
    [DataContract]
    public class TankBarrelNetworkSyncComponent : NetworkSyncComponent
    {
        private float lastRotation;

        [RequiredComponent]
        protected Transform2D transform;

        public override void ReadSyncData(IncomingMessage reader)
        {
            var rotation = reader.ReadSingle();
            this.transform.Rotation = rotation;
        }

        public override bool NeedSendSyncData()
        {
            return this.lastRotation != this.transform.Rotation;
        }

        public override void WriteSyncData(OutgoingMessage writer)
        {
            this.lastRotation = this.transform.Rotation;
            writer.Write(this.lastRotation);
        }
    }
}