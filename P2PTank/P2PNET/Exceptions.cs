using System;

namespace P2PNET
{
    public class PeerNotKnown : Exception
    {
        public PeerNotKnown(string message) : base(message)
        {
        }
    }
    public class StreamCannotWrite : Exception
    {
        public StreamCannotWrite(string message) : base(message)
        {

        }
    }

    public class NoNetworkInterface : Exception
    {
        public NoNetworkInterface(string message) : base(message)
        {

        }
    }

    public class FileNotFound: Exception
    {
        public FileNotFound(string message) : base(message)
        {

        }
    }

    public class FileTransitionError : Exception
    {
        public FileTransitionError(string message) : base(message)
        {

        }
    }

    public class LowLevelTransitionError : Exception
    {
        public LowLevelTransitionError(string message) : base(message)
        {

        }
    }

    public class FileBoundaryException : Exception
    {
        public FileBoundaryException(string message) : base(message)
        {

        }
    }

    public class InvalidMessage : Exception
    {
        public InvalidMessage(string message) : base(message)
        {

        }
    }

    public class DeserializingException : Exception
    {
        public DeserializingException(string message) : base(message)
        {

        }
    }
}
