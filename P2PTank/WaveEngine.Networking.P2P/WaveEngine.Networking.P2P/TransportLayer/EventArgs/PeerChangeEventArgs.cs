using System.Collections.Generic;

namespace WaveEngine.Networking.P2P.TransportLayer.EventArgs
{
    public class PeerPlayerChangeEventArgs : System.EventArgs
    {
        public List<PeerPlayer> Peers { get; }

        public PeerPlayerChangeEventArgs( List<PeerPlayer> peers )
        {
            this.Peers = peers;
        }
    }
}
