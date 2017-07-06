using System;
using System.Threading.Tasks;
using System.IO;
using Sockets.Plugin.Abstractions;
using P2PNET.TransportLayer.EventArgs;
using System.Text;
using System.Linq;

namespace P2PNET.TransportLayer
{
    public class Peer
    {
        public event EventHandler<MsgReceivedEventArgs> MsgReceived;
        public event EventHandler<PeerEventArgs> peerStatusChange;
        public ILogger logger;

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
        public Peer(ITcpSocketClient mSocketClient, ILogger mLogger)
        {
            this.IsPeerActive = true;
            this.socketClient = mSocketClient;
            this.logger = mLogger;
            writeUtil = new WriteStreamUtil(this.socketClient.WriteStream);
            readUtil = new ReadStreamUtil(this.socketClient.ReadStream);

            StartListening();
        }

        public async Task<bool> SendMsgTCPAsync(byte[] msg)
        {
            if(this.IsPeerActive == false)
            {
                //peer has disconnected
                return false;
            }

            await writeUtil.WriteBytesAsync(msg);
            return true;
        }

        private async void StartListening()
        {
            //set timeout for reading
            while (this.IsPeerActive)
            {
                byte[] messageBin = null;
                try
                {
                    messageBin = await readUtil.ReadBytesAsync();
                }
                catch
                {
                    //lost connection with peer
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