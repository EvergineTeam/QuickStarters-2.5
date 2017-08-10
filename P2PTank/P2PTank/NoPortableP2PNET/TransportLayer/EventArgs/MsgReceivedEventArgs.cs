namespace P2PNET.TransportLayer.EventArgs
{
    public class MsgReceivedEventArgs : System.EventArgs
    {
        public byte[] Message { get; }
        public TransportType BindingType { get; }
        public string RemoteIp { get; }

        public MsgReceivedEventArgs(string remoteIp, byte[] msg, TransportType bindType)
        {
            this.RemoteIp = remoteIp;
            this.Message = msg;
            this.BindingType = bindType;
        }
    }
}
