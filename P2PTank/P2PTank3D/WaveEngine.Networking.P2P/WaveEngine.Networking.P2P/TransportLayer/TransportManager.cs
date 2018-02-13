using Sockets.Plugin;
using System;
using System.Collections.Generic;
using Networking.P2P.TransportLayer.EventArgs;
using System.Threading.Tasks;
using System.IO;
using Networking.P2P.Exceptions;
using Sockets.Plugin.Abstractions;

namespace Networking.P2P.TransportLayer
{
    /// <summary>
    /// Low level class that sends and receives messages between peers.
    /// </summary>
    public class TransportManager : IDisposable
    {
        /// <summary>
        /// Triggered when a new peer is detected or an existing peer becomes inactive
        /// </summary>
        public event EventHandler<PeerPlayerChangeEventArgs> PeerPlayerChange;

        /// <summary>
        /// Triggered when a message has been received by this peer
        /// </summary>
        public event EventHandler<MsgReceivedEventArgs> MsgReceived;

        /// <summary>
        /// Selected Interface. If null OS will select first.
        /// </summary>
        public ICommsInterface SelectedCommsInterface { get; set; }

        /// <summary>
        /// A list of all peers that are known to this peer
        /// </summary>
        public List<PeerPlayer> KnownPeers
        {
            get
            {
                return this.baseStation?.KnownPeers;
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
                return listener == null ? false : listener.IsListening;
            }
        }

        /// <summary>
        /// The port number used for sending and receiving messages
        /// </summary>
        public int PortNum { get; }

        public bool tcpOnly { get; }

        private bool forwardAll;
        private Listener listener;
        private BaseStation baseStation;

        /// Constructor that instantiates a transport manager. To commence listening call the method <C>StartAsync</C>.
        /// </summary>
        /// <param name="mPortNum"> The port number which this peer will listen on and send messages with </param>
        /// <param name="mForwardAll"> When true, all messages received trigger a MsgReceived event. This includes UDB broadcasts that are reflected back to the local peer.</param>
        public TransportManager(ICommsInterface commsInterface = null, int mPortNum = 8080, bool mForwardAll = false, bool mTcpOnly = false)
        {
            this.PortNum = mPortNum;
            this.tcpOnly = mTcpOnly;
            this.forwardAll = mForwardAll;

            this.SelectedCommsInterface = commsInterface;
        }


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
            if (IsListening == true)
            {
                //nothing to do
                return;
            }

            if (SelectedCommsInterface == null)
            {
                var interfaces = await CommsInterface.GetAllInterfacesAsync();
                foreach (CommsInterface comms in interfaces)
                {
                    if (IsValidInterface(comms))
                    {
                        this.SelectedCommsInterface = comms;
                        break;
                    }
                }
            }

            if (SelectedCommsInterface == null)
            {
                throw (new NoNetworkInterface("Unable to find an active network interface connection. Is this device connected to wifi?"));
            }

            this.listener = new Listener(this.PortNum, this.SelectedCommsInterface, this.tcpOnly);
            this.baseStation = new BaseStation(this.PortNum, this.forwardAll, this.tcpOnly);

            this.baseStation.PeerPlayerChange += OnBaseStationPeerChange;
            this.baseStation.MsgReceived += IncomingMsg;

            //baseStation looks up incoming messages to see if there is a new peer talk to us
            this.listener.IncomingMsg += this.baseStation.IncomingMsgAsync;
            this.listener.PeerConnectTCPRequest += this.Listener_PeerConnectTCPRequest;

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
            if (listener != null )
            {
                this.listener.IncomingMsg -= this.baseStation.IncomingMsgAsync;
                this.listener.PeerConnectTCPRequest -= this.Listener_PeerConnectTCPRequest;

                listener.Dispose();
                listener = null;
            }

            if (this.baseStation != null)
            {
                this.baseStation.PeerPlayerChange -= OnBaseStationPeerChange;
                this.baseStation.MsgReceived -= IncomingMsg;
                this.baseStation = null;
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
            // Make sure its a unique peer
            foreach (PeerPlayer peer in KnownPeers)
            {
                if (peer.IpAddress == e.SocketClient.RemoteAddress)
                {
                    if (baseStation.forwardAll == false)
                    {
                        return;
                    }
                }
            }
            // Unique peer create a new TCP connection for the peer
            baseStation.NewTCPConnection(sender, e);
        }

        private void IncomingMsg(object sender, MsgReceivedEventArgs e)
        {
            // Send message out
            MsgReceived?.Invoke(this, e);
        }

        private void OnBaseStationPeerChange(object sender, PeerPlayerChangeEventArgs e)
        {
            PeerPlayerChange?.Invoke(this, e);
        }

        //private async Task<string> GetLocalIPAddress()
        //{
        //    if (this.SelectedCommsInterface != null)
        //    {
        //        return this.SelectedCommsInterface.IpAddress;
        //    }
        //    else
        //    {
        //        List<CommsInterface> interfaces = await CommsInterface.GetAllInterfacesAsync();

        //        foreach (CommsInterface comms in interfaces)
        //        {
        //            if (IsValidInterface(comms))
        //            {
        //                return comms.IpAddress;
        //            }
        //        }
        //    }

        //    // Raise exception
        //    throw (new NoNetworkInterface("Unable to find an active network interface connection. Is this device connected to wifi?"));
        //}

        private bool IsValidInterface(CommsInterface commsInterface)
        {
            if (commsInterface.Name.ToLowerInvariant().Equals("wi-fi")
                || commsInterface.Name.ToLowerInvariant().Equals("ethernet")
                || commsInterface.Name.ToLowerInvariant().Contains("vethernet")
                || commsInterface.Name.ToLowerInvariant().Equals("wlan0"))
                if (commsInterface.ConnectionStatus == Sockets.Plugin.Abstractions.CommsInterfaceStatus.Connected)
                    return true;

            return false;
        }

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    CloseConnection();
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            Dispose(true);
        }
    }
}