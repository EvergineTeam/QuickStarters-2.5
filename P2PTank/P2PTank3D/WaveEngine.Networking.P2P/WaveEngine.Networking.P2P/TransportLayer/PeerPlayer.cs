using System;
using System.Threading.Tasks;
using System.IO;
using Sockets.Plugin.Abstractions;
using Networking.P2P.TransportLayer.EventArgs;

namespace Networking.P2P.TransportLayer
{
    public class PeerPlayer
    {
        public event EventHandler<MsgReceivedEventArgs> MsgReceived;
        public event EventHandler<PeerEventArgs> peerStatusChange;

        public string IpAddress
        {
            get
            {
                return socketClient.RemoteAddress;
            }
        }

        public bool IsPeerActive { get; set; }

        public Stream WriteStream
        {
            get
            {
                return writeUtil.ActiveStream;
            }
        }

        public Stream ReadStream
        {
            get
            {
                return readUtil.ActiveStream;
            }
        }

        private WriteStreamUtil writeUtil;
        private ReadStreamUtil readUtil;

        private ITcpSocketClient socketClient;

        //constructor
        public PeerPlayer(ITcpSocketClient mSocketClient)
        {
            this.IsPeerActive = true;
            this.socketClient = mSocketClient;
            writeUtil = new WriteStreamUtil(this.socketClient.WriteStream);
            readUtil = new ReadStreamUtil(this.socketClient.ReadStream);

            StartListening();
        }

        public async Task<bool> SendMsgTCPAsync(byte[] msg)
        {
            if(this.IsPeerActive == false)
            {
                // Disconnected
                return false;
            }

            await writeUtil.WriteBytesAsync(msg);
            return true;
        }

        private async void StartListening()
        {
            // Set timeout for reading
            while (this.IsPeerActive)
            {
                byte[] messageBin = null;
                try
                {
                    messageBin = await readUtil.ReadBytesAsync();
                }
                catch
                {
                    // Lost connection with peer
                    this.IsPeerActive = false;
                    peerStatusChange?.Invoke(this, new PeerEventArgs(this));
                }
                if(IsPeerActive == true)
                {
                    MsgReceived?.Invoke(this, new MsgReceivedEventArgs(socketClient.RemoteAddress, messageBin, TransportType.TCP));
                }
            }
        }
    }
}