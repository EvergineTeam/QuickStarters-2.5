using System;
using Sockets.Plugin;
using Sockets.Plugin.Abstractions;
using System.Threading.Tasks;
using System.Threading;
using Networking.P2P.TransportLayer.EventArgs;

namespace Networking.P2P.TransportLayer
{
    public class Listener : IDisposable
    {
        // Triggered when a peer send a connect request to this peer
        public event EventHandler<TcpSocketListenerConnectEventArgs> PeerConnectTCPRequest;
        public event EventHandler<MsgReceivedEventArgs> IncomingMsg;

        private UdpSocketReceiver listenerUDP;
        private TcpSocketListener listenerTCP;

        private SemaphoreSlim messageReceive = new SemaphoreSlim(0);
        private SemaphoreSlim messageProccessed = new SemaphoreSlim(1);
        private UdpSocketMessageReceivedEventArgs curUDPMessage;

        private SemaphoreSlim tcpConnection = new SemaphoreSlim(0);
        private SemaphoreSlim tcpConnProccessed = new SemaphoreSlim(1);
        private TcpSocketListenerConnectEventArgs curTcpConnection;

        private bool isListening;
        public bool IsListening
        {
            get
            {
                return isListening;
            }
        }

        private int portNum;

        private ICommsInterface commsInterface;

        public Listener(int mPortNum, ICommsInterface commsInterface, bool mTcpOnly = false)
        {
            this.isListening = false;
            this.commsInterface = commsInterface;

            if (!mTcpOnly)
            {
                this.listenerUDP = new UdpSocketReceiver();
            }

            this.listenerTCP = new TcpSocketListener();

            this.portNum = mPortNum;
        }

        public void Dispose()
        {
            listenerUDP?.Dispose();
            listenerTCP.Dispose();
        }

        public async Task StartAsync()
        {
            await StartListeningAsyncTCP(this.portNum);

            if (listenerUDP != null)
            {
                await StartListeningAsyncUDP(this.portNum);
            }
            isListening = true;
        }

        private async Task StartListeningAsyncTCP(int portNum)
        {
            listenerTCP.ConnectionReceived -= ListenerTCP_ConnectionReceived;
            listenerTCP.ConnectionReceived += ListenerTCP_ConnectionReceived;
            await listenerTCP.StartListeningAsync(portNum, commsInterface);

            ListenTcpLoop();
        }

        // This method runs in a separate thread.
        // This is undesirable because windows form elements will complain about shared resources not being avaliable
        // solution is to use a semaphore that is picked up in the other thread
        private async void ListenerTCP_ConnectionReceived(object sender, TcpSocketListenerConnectEventArgs e)
        {
            // Wait until previous connection has been handled
            await tcpConnProccessed.WaitAsync();

            // Update the shared memory
            curTcpConnection = e;

            // Tell main thread of new message (using signal)
            tcpConnection.Release();
        }


        private async void ListenTcpLoop()
        {
            bool tcpListenerActive = true;
            while (tcpListenerActive)
            {
                // Wait until signal is recieved
                TcpSocketListenerConnectEventArgs tcpConnection = await ReceivedIncomingTCP();
                PeerConnectTCPRequest?.Invoke(this, tcpConnection);
            }
        }
        private async Task<TcpSocketListenerConnectEventArgs> ReceivedIncomingTCP()
        {
            // Wait until recieved signal
            TcpSocketListenerConnectEventArgs tempConnection;
            try
            {
                await tcpConnection.WaitAsync();
                tempConnection = curTcpConnection;
            }
            finally
            {
                // Ready to recieve next message
                tcpConnProccessed.Release();
            }
            return tempConnection;
        }


        private async Task StartListeningAsyncUDP(int portNum)
        {
            if (this.listenerUDP == null)
            {
                throw new InvalidOperationException("Cannot listen on UDP when in TCP only mode.");
            }

            this.listenerUDP.MessageReceived += this.ListenerUDP_MessageReceived;
            await this.listenerUDP.StartListeningAsync(this.portNum, this.commsInterface);

            this.ListenUdpLoop();
        }

        // This method runs in a seperate thread.
        // This is undesirable because windows form elements will complain about shared resources not being avaliable
        // solution is to use a semaphore that is picked up in the other thread
        private async void ListenerUDP_MessageReceived(object sender, UdpSocketMessageReceivedEventArgs e)
        {
            // Wait until previous message has been handled
            await messageProccessed.WaitAsync();

            // Update the shared memory
            curUDPMessage = e;

            // Tell main thread of new message (using signal)
            messageReceive.Release();
        }

        private async void ListenUdpLoop()
        {
            bool udpListenerActive = true;
            while (udpListenerActive)
            {
                // Wait until signal is recieved
                UdpSocketMessageReceivedEventArgs udpMsg = await MessageReceivedUdp();
                IncomingMsg?.Invoke(this, new MsgReceivedEventArgs(udpMsg.RemoteAddress, udpMsg.ByteData, TransportType.UDP));
            }
        }

        private async Task<UdpSocketMessageReceivedEventArgs> MessageReceivedUdp()
        {
            // Wait until recieved signal
            UdpSocketMessageReceivedEventArgs tempMsgHandler;
            try
            {
                await messageReceive.WaitAsync();
                tempMsgHandler = curUDPMessage;
            }
            finally
            {
                // Ready to recieve next message
                messageProccessed.Release();
            }
            return tempMsgHandler;
        }
    }
}
