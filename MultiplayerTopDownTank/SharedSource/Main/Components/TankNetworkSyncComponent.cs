using System.Runtime.Serialization;
using WaveEngine.Networking.Messages;

namespace MultiplayerTopDownTank.Components
{
    [DataContract]
    public class TankNetworkSyncComponent : BaseNetworkSyncComponent
    {
        private float lastRotation;

        public override void ReadSyncData(IncomingMessage reader)
        {
            base.ReadSyncData(reader);

            var rotation = reader.ReadSingle();
            this.transform.Rotation = rotation;
        }


        public override void WriteSyncData(OutgoingMessage writer)
        {
            base.WriteSyncData(writer);
      
            this.lastRotation = this.transform.Rotation;
            writer.Write(this.lastRotation);
        }
    }
}