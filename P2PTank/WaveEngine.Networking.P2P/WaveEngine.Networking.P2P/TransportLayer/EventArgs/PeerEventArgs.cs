namespace WaveEngine.Networking.P2P.TransportLayer.EventArgs
{
    public class PeerEventArgs : System.EventArgs
    {
        public PeerPlayer Peer { get; }

        public PeerEventArgs(PeerPlayer peer)
        {
            this.Peer = peer;
        }
    }
}
