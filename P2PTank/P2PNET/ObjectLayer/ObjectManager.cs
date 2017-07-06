#if OBJECT
using System;
using System.Threading.Tasks;
using P2PNET.TransportLayer;
using P2PNET.TransportLayer.EventArgs;
using P2PNET.ObjectLayer.EventArgs;
using System.Collections.Generic;

namespace P2PNET.ObjectLayer
{
    /// <summary>
    /// Class for sending and receiving objects between peers.
    /// Built on top of TransportManager.
    /// </summary>
    public class ObjectManager: IDisposable
    {
        /// <summary>
        /// Triggered when a new peer is detected or an existing peer becomes inactive
        /// </summary>
        public event EventHandler<PeerChangeEventArgs> PeerChange;

        /// <summary>
        /// Triggered when a message containing an object has been received
        /// </summary>
        public event EventHandler<ObjReceivedEventArgs> ObjReceived;

        /// <summary>
        /// A list of all peers that are known to this peer
        /// </summary>
        public List<Peer> KnownPeers
        {
            get
            {
                return peerManager.KnownPeers;
            }
        }

        private Serializer serializer;
        private TransportManager peerManager;

        /// <summary>
        /// Constructor that instantiates a object manager. To commence listening call the method <C>StartAsync</C>.
        /// </summary>
        /// <param name="mPortNum"> The port number which this peer will listen on and send messages with </param>
        /// <param name="mForwardAll"> When true, all messages received trigger a MsgReceived event. This includes UDB broadcasts that are reflected back to the local peer.</param>
        public ObjectManager(int mPortNum = 8080, bool mForwardAll = false, bool tcpOnly = false, ILogger mLogger = null)
        {
            peerManager = new TransportManager(mPortNum, mForwardAll, tcpOnly, mLogger);
            serializer = new Serializer();

            peerManager.MsgReceived += PeerManager_msgReceived;
            peerManager.PeerChange += PeerManager_PeerChange;
        }

        /// <summary>
        /// Peer will start actively listening on the specified port number.
        /// </summary>
        /// <returns></returns>
        public async Task StartAsync()
        {
            await peerManager.StartAsync();
        }
        
        /// <summary>
        /// Sends an object to a peer via a reliable TCP connection
        /// </summary>
        /// <param name="ipAddress"> the IPv4 address to send the message to. In the format "xxxx.xxxx.xxxx.xxxx" </param>
        /// <param name="obj">The object to send to the other peer</param>
        /// <returns>true if message was sucessfully sent otherwise returns false</returns>
        public async Task<bool> SendAsyncTCP<T>(string ipAddress, T obj)
        {
            if(obj == null)
            {
                //no object to send
                return false;
            }
            byte[] msg = await PackObjectIntoMsg(obj);

            return await peerManager.SendAsyncTCP(ipAddress, msg);
        }

        /// <summary>
        /// Sends an object to a peer via an unreliable UDP connection
        /// Use <C>SendAsyncTCP</C> instead if packet loss cannot be tolerated. 
        /// </summary>
        /// <param name="ipAddress"> the IPv4 address to send the message to. In the format "xxxx.xxxx.xxxx.xxxx" </param>
        /// <param name="obj">The object to send to the other peer</param>
        /// <returns>true if message was sucessfully sent otherwise returns false</returns>
        public async Task<bool> SendAsyncUDP<T>(string ipAddress, T obj)
        {
            byte[] msg = await PackObjectIntoMsg(obj);

            return await peerManager.SendAsyncUDP(ipAddress, msg);
        }

        /// <summary>
        /// Sends an unreliable UDP broadcast to the local router. Depending on your local router settings UDP broadcasts may be ignored.
        /// If the address of other peers is known use <C>SendToAllPeersAsyncUDP</C> instead.
        /// </summary>
        /// <param name="obj">The object that is broadcast to other peers</param>
        /// <returns></returns>
        public async Task SendBroadcastAsyncUDP<T>(T obj)
        {
            byte[] msg = await PackObjectIntoMsg(obj);

            await peerManager.SendBroadcastAsyncUDP(msg);
        }

        /// <summary>
        /// Sends a message via unreliable UDP to all known peers.
        /// Use <C>SendToAllPeersAsyncTCP</C> instead if packet loss can not be tolerated. 
        /// </summary>
        /// <param name="obj">The object that is sent to all other peers</param>
        /// <returns></returns>
        public async Task SendToAllPeersAsyncUDP<T>(T obj)
        {
            byte[] msg = await PackObjectIntoMsg(obj);

            await peerManager.SendToAllPeersAsyncUDP(msg);
        }

        /// <summary>
        /// Sends a message via reliable TCP connections to all known peers.
        /// </summary>
        /// <param name="obj">The object that is sent to all other peers</param>
        /// <returns></returns>
        public async Task SendToAllPeersAsyncTCP<T>(T obj)
        {
            byte[] msg = await PackObjectIntoMsg(obj);

            await peerManager.SendToAllPeersAsyncTCP(msg);
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
            await peerManager.DirectConnectAsyncTCP(ipAddress);
        }

        private async Task<byte[]> PackObjectIntoMsg<T>(T obj)
        {
            //generate metadata
            Metadata metadata = await CreateMetadataObj(obj);

            //place object into a package
            ObjPackage<T> objPackage = new ObjPackage<T>( obj, metadata);

            //seralize package
            byte[] objMsg = serializer.SerializeObject(objPackage);

            return objMsg;
        }

        private async Task<Metadata> CreateMetadataObj<T>(T obj)
        {
            string sourceIp = await peerManager.GetIpAddress();
            string objType = obj.GetType().Name;
            Metadata metaData = new Metadata();
            metaData.SourceIp = sourceIp;
            metaData.ObjectType = objType;
            return metaData;
        }

        private void PeerManager_PeerChange(object sender, TransportLayer.EventArgs.PeerChangeEventArgs e)
        {
            PeerChange?.Invoke(this, e);
        }

        private void PeerManager_msgReceived(object sender, TransportLayer.EventArgs.MsgReceivedEventArgs e)
        {
            byte[] msg = e.Message;
            BObject obj = new BObject(msg, serializer);

            //generate metadata
            Metadata metadata = obj.GetMetadata();
            metadata.BindType = e.BindingType;
            ObjReceived?.Invoke(this, new ObjReceivedEventArgs(obj, metadata));
        }

        public void Dispose()
        {
            ((IDisposable)peerManager).Dispose();
        }
    }
}

#endif