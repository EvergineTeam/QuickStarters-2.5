using System.Collections.Generic;


namespace P2PNET.TransportLayer.EventArgs
{
    public class PeerEventArgs : System.EventArgs
    {
        public Peer Peer { get; }

        //constructor
        public PeerEventArgs( Peer peer )
        {
            this.Peer = peer;
        }
    }
}
