#if MESSAGE
using Sockets.Plugin;
using System;
using System.Collections.Generic;
using P2PNET.TransportLayer.EventArgs;
using System.Threading.Tasks;
using System.IO;

namespace P2PNET.TransportLayer
{
    /// <summary>
    /// Low level class that sends and receives messages between peers.
    /// </summary>
    public class TransportManager: IDisposable
    {
        /// <summary>
        /// Triggered when a new peer is detected or an existing peer becomes inactive
        /// </summary>
        public event EventHandler<PeerChangeEventArgs> PeerChange;

        /// <summary>
        /// Triggered when a message has been received by this peer
        /// </summary>
        public event EventHandler<MsgReceivedEventArgs> MsgReceived;


        /// <summary>
        /// A list of all peers that are known to this peer
        /// </summary>
        public List<Peer> KnownPeers
        {
            get
            {
                return this.baseStation.KnownPeers;
            }
        }

        /// <summary>
        /// true = listening for incoming messages
        /// false = not actively listening for incoming messages
        /// </summary>
        public bool IsListening 
        {
            get
            {
                return listener.IsListening;
            } 
        }

        private string ipAddress = null;

        /// <summary>
        /// Get methods that retreives the IPv4 address of the local peer asynchronously
        /// </summary>
        /// <returns> A string in the format xxxx.xxxx.xxxx.xxxx  </returns>
        public async Task<string> GetIpAddress()
        {
            if(ipAddress == null)
            {
                ipAddress = await GetLocalIPAddress();
            }
            return ipAddress;
        }

        /// <summary>
        /// The port number used for sending and receiving messages
        /// </summary>
        public int PortNum { get; }
        public bool tcpOnly { get; }

        private Listener listener;
        private BaseStation baseStation;

        /// Constructor that instantiates a transport manager. To commence listening call the method <C>StartAsync</C>.
        /// </summary>
        /// <param name="mPortNum"> The port number which this peer will listen on and send messages with </param>
        /// <param name="mForwardAll"> When true, all messages received trigger a MsgReceived event. This includes UDB broadcasts that are reflected back to the local peer.</param>
        public TransportManager(int mPortNum = 8080, bool mForwardAll = false, bool mTcpOnly = false, ILogger mLogger = null)
        {
            this.PortNum = mPortNum;
            this.tcpOnly = mTcpOnly;
            this.listener = new Listener(this.PortNum, mTcpOnly);
            this.baseStation = new BaseStation(this.PortNum, mForwardAll, mTcpOnly, mLogger);

            this.baseStation.PeerChange += BaseStation_PeerChange;
            this.baseStation.MsgReceived += IncomingMsg;

            //baseStation looks up incoming messages to see if there is a new peer talk to us
            this.listener.IncomingMsg += baseStation.IncomingMsgAsync;
            this.listener.PeerConnectTCPRequest += Listener_PeerConnectTCPRequest;
        }

        
        //deconstructor
        ~TransportManager()
        {
            this.CloseConnection();
        }
        

        /// <summary>
        /// Peer will start actively listening on the specified port number.
        /// </summary>
        /// <returns></returns>
        public async Task StartAsync()
        {
            //check if already started
            if ( IsListening == true )
            {
                //nothing to do
                return;
            }

            baseStation.LocalIpAddress = await GetLocalIPAddress();
            await listener.StartAsync();
        }

        /// <summary>
        /// Peer will destroy all connections. All known peers will be cleared.
        /// I have been thinking about making this method public however I don't want users
        /// to use it to temporary stop listening. It should be used to terminate all connections
        /// at the end of an application.
        /// </summary>
        /// <returns></returns>
        private void CloseConnection()
        {
            if( listener == null)
            {
                listener.Dispose();
                listener = null;
            }

            KnownPeers.Clear();
        }

        /// <summary>
        /// Sends a message to a peer via a reliable TCP connection
        /// </summary>
        /// <param name="ipAddress"> the IPv4 address to send the message to. In the format "xxxx.xxxx.xxxx.xxxx" </param>
        /// <param name="msg">The message to send to the other peer</param>
        /// <returns>true if message was sucessfully sent otherwise returns false</returns>
        public async Task<bool> SendAsyncTCP(string ipAddress, byte[] msg)
        {
            return await baseStation.SendTCPMsgAsync(ipAddress, msg);
        }

        /// <summary>
        /// Sends a message to a peer via an unreliable UDP connection.
        /// Use <C>SendAsyncTCP</C> instead if packet loss cannot be tolerated. 
        /// </summary>
        /// <param name="ipAddress"> the IPv4 address to send the message to. In the format "xxxx.xxxx.xxxx.xxxx" </param>
        /// <param name="msg">The message to send to the other peer</param>
        /// <returns>true if message was sucessfully sent otherwise returns false</returns>
        public async Task<bool> SendAsyncUDP(string ipAddress, byte[] msg)
        {
            return await baseStation.SendUDPMsgAsync(ipAddress, msg);
        }

        /// <summary>
        /// Sends an unreliable UDP broadcast to the local router. Depending on your local router settings UDP broadcasts may be ignored.
        /// If the address of other peers is known use <C>SendToAllPeersAsyncUDP</C> instead.
        /// </summary>
        /// <param name="msg">The message broadcast to other peers</param>
        /// <returns></returns>
        public async Task SendBroadcastAsyncUDP(byte[] msg)
        {
            await baseStation.SendUDPBroadcastAsync(msg);
        }

        /// <summary>
        /// Sends a message via unreliable UDP to all known peers.
        /// Use <C>SendToAllPeersAsyncTCP</C> instead if packet loss can not be tolerated. 
        /// </summary>
        /// <param name="msg">The message sent to all other peers</param>
        /// <returns></returns>
        public async Task SendToAllPeersAsyncUDP(byte[] msg)
        {
            await baseStation.SendUDPMsgToAllTCPAsync(msg);
        }

        /// <summary>
        /// Sends a message via reliable TCP connections to all known peers.
        /// </summary>
        /// <param name="msg">The message sent to all other peers</param>
        /// <returns></returns>
        public async Task SendToAllPeersAsyncTCP(byte[] msg)
        {
            await baseStation.SendTCPMsgToAllTCPAsync(msg);
        }

        //This is here for existing Peer to Peer systems that use asynchronous Connections.
        //This method is not needed otherwise because this class automatically keeps track
        //of peer connections
        /// <summary>
        /// This method is avaliable to make it easier to integrate existing asymetric peer to peer systems.
        /// </summary>
        /// <param name="ipAddress">the ip address to establish a connection with</param>
        /// <returns></returns>
        public async Task DirectConnectAsyncTCP(string ipAddress)
        {
            await baseStation.DirectConnectTCPAsync(ipAddress);
        }

        /// <summary>
        /// returns a TCP stream that is used for sending data to the given ip address.
        /// This is for systems that require a TCP stream. Consider using the method SendAsyncTCP() instead.
        /// Make sure to flush the stream to make sure the data is sent.
        /// </summary>
        /// <param name="ipAddress"> the target ip address you want to connect to </param>
        /// <returns>A TCP stream that can be written to</returns>
        public async Task<Stream> GetWriteStreamAsync(string ipAddress)
        {
            return await baseStation.GetWriteStreamAsync(ipAddress);
        }

        /// <summary>
        /// returns a TCP stream that is used for receiving data from the given ip address.
        /// This is for systems that require a TCP stream. Consider using the event handler MsgReceived instead.
        /// </summary>
        /// <param name="ipAddress"> the target ip address you want to connect to </param>
        /// <returns>A TCP stream that can be read from</returns>
        public async Task<Stream> GetReadStreamAsync(string ipAddress)
        {
            return await baseStation.GetReadStreamAsync(ipAddress);
        }

        private void Listener_PeerConnectTCPRequest(object sender, Sockets.Plugin.Abstractions.TcpSocketListenerConnectEventArgs e)
        {
            //make sure its a unique peer
            foreach (Peer peer in KnownPeers)
            {
                if (peer.IpAddress == e.SocketClient.RemoteAddress)
                {
                    if( baseStation.forwardAll == false)
                    {
                        return;
                    }
                }
            }
            //unique peer create a new TCP connection for the peer
            baseStation.NewTCPConnection(sender, e);
        }

        private void IncomingMsg(object sender, MsgReceivedEventArgs e)
        {
            //send message out
            MsgReceived?.Invoke(this, e);
        }

        private void BaseStation_PeerChange(object sender, PeerChangeEventArgs e)
        {
            PeerChange?.Invoke(this, e);
        }

        private async Task<string> GetLocalIPAddress()
        {
            List<CommsInterface> interfaces = await CommsInterface.GetAllInterfacesAsync();
            foreach(CommsInterface comms in interfaces)
            {
                if(IsValidInterface(comms))
                {
                    return comms.IpAddress;
                }
            }

            //raise exception
            throw (new NoNetworkInterface("Unable to find an active network interface connection. Is this device connected to wifi?"));
        }

        private bool IsValidInterface(CommsInterface commsInterface)
        {
            if (commsInterface.Name.ToLowerInvariant().Equals("wi-fi") 
                || commsInterface.Name.ToLowerInvariant().Equals("ethernet"))
                if (commsInterface.ConnectionStatus == Sockets.Plugin.Abstractions.CommsInterfaceStatus.Connected)
                    return true;

            return false;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    CloseConnection();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~TransportManager() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
#endif