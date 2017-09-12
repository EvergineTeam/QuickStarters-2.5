using System;

namespace WaveEngine.Networking.P2P.Exceptions
{
    public class PeerNotKnown : Exception
    {
        // Unknown player
        public PeerNotKnown(string message) : base(message)
        {
        }
    }
}