using P2PNET.TransportLayer.EventArgs;
using Sockets.Plugin;
using Sockets.Plugin.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace P2PNET.TransportLayer
{
    public class BaseStation
    {
        public event EventHandler<PeerChangeEventArgs> PeerChange;
        public event EventHandler<MsgReceivedEventArgs> MsgReceived;
        public ILogger logger;

        public List<Peer> KnownPeers {
            get
            {
                return knownPeers;
            }
        }

        public string LocalIpAddress { get; set; }

        private List<Peer> knownPeers;
        public readonly int portNum;
        public readonly bool tcpOnly;
        public bool forwardAll;

        private UdpSocketClient senderUDP;

        //constructor
        public BaseStation(int mPortNum, bool mForwardAll = false, bool mTcpOnly = false, ILogger mLogger = null)
        {
            this.knownPeers = new List<Peer>();
            if (!mTcpOnly)
                this.senderUDP = new UdpSocketClient();
            this.logger = mLogger;

            this.forwardAll = mForwardAll;
            this.portNum = mPortNum;
            this.tcpOnly = mTcpOnly;
        }

        public async Task<bool> SendUDPMsgAsync(string ipAddress, byte[] msg)
        {
            if (tcpOnly) return false;

            bool isPeerKnown = DoesPeerExistByIp(ipAddress);
            if(!isPeerKnown)
            {
                try
                {
                    await DirectConnectTCPAsync(ipAddress);
                }
                catch
                {
                    throw new PeerNotKnown("The peer is not known");
                }
            }
            await senderUDP.SendToAsync(msg, ipAddress, this.portNum);
            return true;
        }

        public async Task SendUDPBroadcastAsync(byte[] msg)
        {
            if (tcpOnly)
                throw new InvalidOperationException("Operating in TCP-only mode - cannot send or receive UDP");

            string brdcstAddress = "255.255.255.255";
            await senderUDP.SendToAsync(msg, brdcstAddress, this.portNum);
        }

        public async Task SendUDPMsgToAllTCPAsync(byte[] msg)
        {
            if (tcpOnly)
                throw new InvalidOperationException("Operating in TCP-only mode - cannot send or receive UDP");

            foreach (Peer peer in knownPeers)
            {
                string curIpAddress = peer.IpAddress;
                bool isActive = peer.IsPeerActive;
                if (isActive)
                {
                    await SendUDPMsgAsync(curIpAddress, msg);
                }
            }
        }

        //returns false if was unsuccessful
        //thinking about throwing an exception instead
        public async Task<bool> SendTCPMsgAsync(string ipAddress, byte[] msg)
        {
            //check if ipAddress is from this peer
            if(this.LocalIpAddress == ipAddress)
            {
                if(!forwardAll)
                {
                    throw (new PeerNotKnown("The ipAddress your have entered does not correspond to a valid Peer. Check the IP address"));
                }
            }

            //check if message is not null
            if (msg == null || msg.Length <= 0)
            {
                throw new InvalidMessage("The message you are trying to send is null.");
            }

            //check if from unknown peer
            bool peerKnown = DoesPeerExistByIp(ipAddress);
            if (!peerKnown)
            {
                //ipaddress is unknown
                //try to establish an connection with this ipAddress
                try
                {
                    await DirectConnectTCPAsync(ipAddress);
                }
                catch( Exception )
                {
                    throw (new PeerNotKnown("The ip address your have entered does not correspond to a valid Peer. Check the IP address."));
                }
            }

            int indexNum = FindPeerByIp(ipAddress);
            //make sure peer is active
            if (!this.KnownPeers[indexNum].IsPeerActive)
            {
                //peer not active
                return false;
            }
            //logger.WriteLine(Encoding.UTF8.GetString(msg, 0, msg.Length));
            return await this.KnownPeers[indexNum].SendMsgTCPAsync(msg);

        }

        public async Task SendTCPMsgToAllTCPAsync(byte[] msg)
        {
            foreach(Peer peer in knownPeers)
            {
                string curIpAddress = peer.IpAddress;
                bool isActive = peer.IsPeerActive;
                if(isActive)
                {
                    await SendTCPMsgAsync(curIpAddress, msg);
                }
            }
        }

        public async void IncomingMsgAsync(object sender, MsgReceivedEventArgs e)
        {
            //check if message is from this peer
            if(e.RemoteIp == this.LocalIpAddress)
            {
                //from this peer.
                //no futher proccessing needed
                if(forwardAll == false)
                {
                    return;
                }
            }

            //check if its from a new peer
            if(e.BindingType == TransportType.UDP)
            {
                string remotePeeripAddress = e.RemoteIp;
                bool peerKnown = DoesPeerExistByIp(remotePeeripAddress);
                if (!peerKnown)
                {
                    //not a known peer
                    await DirectConnectTCPAsync(remotePeeripAddress);
                }
            }

            //check if its a blank UDP packet
            //These are used as heart beats
            if(e.Message.Length <= 0)
            {
                return;
            }

            //trigger sent message
            MsgReceived?.Invoke(this, e);
        }

        public void NewTCPConnection(object sender, TcpSocketListenerConnectEventArgs e)
        {
            StoreConnectedPeerTCP(e.SocketClient);
        }

        public async Task DirectConnectTCPAsync(string ipAddress)
        {
            //send connection request
            TcpSocketClient senderTCP = new TcpSocketClient();

            //if you get an error on the line below then the person you trying to connect to
            //hasn't accepted in the incoming connection
            try
            {
                await senderTCP.ConnectAsync(ipAddress, this.portNum);
            }
            catch( Exception e1)
            {
                throw e1;
            }
            ITcpSocketClient socketClient = senderTCP;
            StoreConnectedPeerTCP(socketClient);
        }

        public async Task<Stream> GetWriteStreamAsync(string ipAddress)
        {
            bool peerKnown = DoesPeerExistByIp(ipAddress);
            if(!peerKnown)
            {
                //try to establish an connection with this ipAddress
                try
                {
                    await DirectConnectTCPAsync(ipAddress);
                }
                catch (Exception)
                {
                    throw (new PeerNotKnown("The ip address your have entered does not correspond to a valid Peer. Check the IP address."));
                }
            }

            int peerIndex = FindPeerByIp(ipAddress);

            if(peerIndex < 0)
            {
                throw new PeerNotKnown("The peer does not exist");
                //ipaddress is unknown                
            }

            return knownPeers[peerIndex].WriteStream;
        }

        public async Task<Stream> GetReadStreamAsync(string ipAddress)
        {
            bool peerKnown = DoesPeerExistByIp(ipAddress);
            if (!peerKnown)
            {
                //try to establish an connection with this ipAddress
                try
                {
                    await DirectConnectTCPAsync(ipAddress);
                }
                catch (Exception)
                {
                    throw (new PeerNotKnown("The ip address your have entered does not correspond to a valid Peer. Check the IP address."));
                }
            }

            int peerIndex = FindPeerByIp(ipAddress);

            if (peerIndex < 0)
            {
                throw new PeerNotKnown("The peer does not exist");
            }

            return knownPeers[peerIndex].ReadStream;
        }

        private void StoreConnectedPeerTCP( ITcpSocketClient socketClient )
        {
            Peer newPeer = new Peer(socketClient, logger);
            newPeer.MsgReceived += NewPeer_MsgReceived;
            newPeer.peerStatusChange += NewPeer_peerStatusChange;
            knownPeers.Add(newPeer);

            //tell others there is a new peer
            PeerChange?.Invoke(this, new PeerChangeEventArgs(knownPeers));
        }

        private void NewPeer_peerStatusChange(object sender, PeerEventArgs e)
        {
            Peer changedPeer = e.Peer;

            //delete inactive peers
            bool isPeerActive = changedPeer.IsPeerActive;
            if(!isPeerActive)
            {
                //delete from list
                knownPeers.Remove(changedPeer);
            }

            //tell others a peer has been deleted
            PeerChange?.Invoke(this, new PeerChangeEventArgs(knownPeers));
        }

        private void NewPeer_MsgReceived(object sender, MsgReceivedEventArgs e)
        {
            MsgReceived?.Invoke(this, e);
        }

        //returns true if the ip address corresponds to known peer.
        private bool DoesPeerExistByIp(string ipAddress)
        {
            /*
            if(this.LocalIpAddress == ipAddress)
            {
                //From local peer
                return true;
            }
            */
            int count = this.knownPeers.Count;
            for (int i = 0; i < count; ++i)
            {
                if(this.knownPeers[i]?.IpAddress == ipAddress)
                {
                    return true;
                }
            }
            return false;
        }

        //returns -1 if can't find peer
        private int FindPeerByIp(string ipAddress)
        {
            for (int i = 0; i < this.knownPeers.Count; ++i)
            {
                if (this.knownPeers[i].IpAddress == ipAddress)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
