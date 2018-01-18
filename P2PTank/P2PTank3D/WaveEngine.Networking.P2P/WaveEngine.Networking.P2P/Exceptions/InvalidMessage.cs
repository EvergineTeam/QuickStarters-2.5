using System;

namespace Networking.P2P.Exceptions
{
    public class InvalidMessage : Exception
    {
        public InvalidMessage(string message) : base(message)
        {

        }
    }
}