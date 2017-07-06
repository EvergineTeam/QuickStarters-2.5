using System;
using P2PNET.TransportLayer.EventArgs;
using Sockets.Plugin;
using Sockets.Plugin.Abstractions;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace P2PNET.TransportLayer
{
    public class Listener : IDisposable
    {
        //triggered when a peer send a connect request to this peer
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

        //constructor
        public Listener(int mPortNum, bool mTcpOnly=false)
        {
            isListening = false;

            if (!mTcpOnly)
                this.listenerUDP = new UdpSocketReceiver();
            this.listenerTCP = new TcpSocketListener();

            this.portNum = mPortNum;
        }

        //destory connection
        public void Dispose()
        {
            listenerUDP?.Dispose();
            listenerTCP.Dispose();
        }

        public async Task StartAsync()
        {
            await StartListeningAsyncTCP(this.portNum);
            if (listenerUDP != null)
                await StartListeningAsyncUDP(this.portNum);
            isListening = true;
        }

        private async Task StartListeningAsyncTCP(int portNum)
        {
            listenerTCP.ConnectionReceived += ListenerTCP_ConnectionReceived;
            await listenerTCP.StartListeningAsync(portNum);

            ListenTcpLoop();
        }

        //This method runs in a seperate thread.
        //This is undesirable because windows form elements will complain about shared resources not being avaliable
        //solution is to use a semaphore that is picked up in the other thread
        private async void ListenerTCP_ConnectionReceived(object sender, TcpSocketListenerConnectEventArgs e)
        {
            //wait until previous connection has been handled
            await tcpConnProccessed.WaitAsync();

            //update the shared memory
            curTcpConnection = e;

            //tell main thread of new message (using signal)
            tcpConnection.Release();
        }


        private async void ListenTcpLoop()
        {
            bool tcpListenerActive = true;
            while (tcpListenerActive)
            {
                //wait until signal is recieved
                TcpSocketListenerConnectEventArgs tcpConnection = await ReceivedIncomingTCP();
                PeerConnectTCPRequest?.Invoke(this, tcpConnection);
            }
        }
        private async Task<TcpSocketListenerConnectEventArgs> ReceivedIncomingTCP()
        {
            //TODO
            //wait until recieved signal
            TcpSocketListenerConnectEventArgs tempConnection;
            try
            {
                await tcpConnection.WaitAsync();
                tempConnection = curTcpConnection;
            }
            finally
            {
                //ready to recieve next message
                tcpConnProccessed.Release();
            }
            return tempConnection;
        }


        private async Task StartListeningAsyncUDP(int portNum)
        {
            if (listenerUDP == null)
                throw new InvalidOperationException("Cannot listen on UDP when in TCP only mode.");

            listenerUDP.MessageReceived += ListenerUDP_MessageReceived;
            await listenerUDP.StartListeningAsync(portNum);

            ListenUdpLoop();
        }

        //This method runs in a seperate thread.
        //This is undesirable because windows form elements will complain about shared resources not being avaliable
        //solution is to use a semaphore that is picked up in the other thread
        private async void ListenerUDP_MessageReceived(object sender, UdpSocketMessageReceivedEventArgs e)
        {
            //wait until previous message has been handled
            await messageProccessed.WaitAsync();

            //update the shared memory
            curUDPMessage = e;

            //tell main thread of new message (using signal)
            messageReceive.Release();
        }

        private async void ListenUdpLoop()
        {
            bool udpListenerActive = true;
            while (udpListenerActive)
            {
                //wait until signal is recieved
                UdpSocketMessageReceivedEventArgs udpMsg = await MessageReceivedUdp();
                IncomingMsg?.Invoke(this, new MsgReceivedEventArgs(udpMsg.RemoteAddress, udpMsg.ByteData, TransportType.UDP));
            }
        }

        private async Task<UdpSocketMessageReceivedEventArgs> MessageReceivedUdp()
        {
            //wait until recieved signal
            UdpSocketMessageReceivedEventArgs tempMsgHandler;
            try
            {
                await messageReceive.WaitAsync();
                tempMsgHandler = curUDPMessage;
            }
            finally
            {
                //ready to recieve next message
                messageProccessed.Release();
            }
            return tempMsgHandler;
        }
    }
}
