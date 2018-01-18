using System;

namespace Networking.P2P.Exceptions
{
    public class NoNetworkInterface : Exception
    {
        // NoNetworkInterface provides configuration and statistical information for a network interface.
        // No found NoNetworkInterface Exception.
        public NoNetworkInterface(string message) : base(message)
        {

        }
    }
}