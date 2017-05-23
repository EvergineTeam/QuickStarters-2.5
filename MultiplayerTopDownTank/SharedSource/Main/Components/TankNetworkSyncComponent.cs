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
    public class TankNetworkSyncComponent : NetworkSyncComponent
    {
        private Transform2D barrelTransform;
        private Vector2 lastPosition;
        private float lastRotation;
        private float lastBarrelRotation;

        [RequiredComponent]
        protected Transform2D transform;

        [RequiredComponent]
        protected RigidBody2D rigidBody;

        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            var barrel = this.Owner.FindChild(GameConstants.PlayerBarrel);
            this.barrelTransform = barrel.FindComponent<Transform2D>();
        }

        public override bool NeedSendSyncData()
        {
            return
                this.lastPosition != this.transform.Position ||
                this.lastRotation != this.transform.Rotation ||
                this.lastBarrelRotation != this.barrelTransform.Rotation;
        }

        public override void ReadSyncData(IncomingMessage reader)
        {
            var x = reader.ReadSingle();
            var y = reader.ReadSingle();
            this.transform.Position = new Vector2(x, y);

            var rotation = reader.ReadSingle();
            this.transform.Rotation = rotation;

            this.rigidBody.ResetPosition(new Vector2(x, y), -rotation);

            var barrelRotation = reader.ReadSingle();
            this.barrelTransform.Rotation = barrelRotation;
        }

        public override void WriteSyncData(OutgoingMessage writer)
        {
            this.lastPosition = this.transform.Position;
            writer.Write(this.lastPosition.X);
            writer.Write(this.lastPosition.Y);

            this.lastRotation = this.transform.Rotation;
            writer.Write(this.lastRotation);

            this.lastBarrelRotation = this.barrelTransform.Rotation;
            writer.Write(this.lastBarrelRotation);
        }
    }
}