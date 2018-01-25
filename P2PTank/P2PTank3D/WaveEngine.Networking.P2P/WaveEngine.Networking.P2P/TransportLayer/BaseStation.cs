using Sockets.Plugin;
using Sockets.Plugin.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Networking.P2P.Exceptions;
using Networking.P2P.TransportLayer.EventArgs;

namespace Networking.P2P.TransportLayer
{
    public class BaseStation
    {
        public event EventHandler<PeerPlayerChangeEventArgs> PeerPlayerChange;
        public event EventHandler<MsgReceivedEventArgs> MsgReceived;

        public List<PeerPlayer> KnownPeers {
            get
            {
                return knownPeers;
            }
        }

        public string LocalIpAddress { get; set; }

        private List<PeerPlayer> knownPeers;
        public readonly int portNum;
        public readonly bool tcpOnly;
        public bool forwardAll;

        private UdpSocketClient senderUDP;

        public BaseStation(int mPortNum, bool mForwardAll = false, bool mTcpOnly = false)
        {
            this.knownPeers = new List<PeerPlayer>();

            if (!mTcpOnly)
                this.senderUDP = new UdpSocketClient();

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

            foreach (PeerPlayer peer in knownPeers)
            {
                string curIpAddress = peer.IpAddress;
                bool isActive = peer.IsPeerActive;
                if (isActive)
                {
                    await SendUDPMsgAsync(curIpAddress, msg);
                }
            }
        }

        // Returns false if was unsuccessful
        public async Task<bool> SendTCPMsgAsync(string ipAddress, byte[] msg)
        {
            // Check if ipAddress is from this peer
            if(this.LocalIpAddress == ipAddress)
            {
                if(!forwardAll)
                {
                    throw (new PeerNotKnown("The ipAddress your have entered does not correspond to a valid Peer. Check the IP address"));
                }
            }

            // Check if message is not null
            if (msg == null || msg.Length <= 0)
            {
                throw new InvalidMessage("The message you are trying to send is null.");
            }

            // Check if from unknown peer
            bool peerKnown = DoesPeerExistByIp(ipAddress);
            if (!peerKnown)
            {
                // ipaddress is unknown
                // Try to establish an connection with this ipAddress
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
            // Make sure peer is active
            if (!this.KnownPeers[indexNum].IsPeerActive)
            {
                // Peer not active
                return false;
            }

            return await this.KnownPeers[indexNum].SendMsgTCPAsync(msg);

        }

        public async Task SendTCPMsgToAllTCPAsync(byte[] msg)
        {
            foreach(PeerPlayer peer in knownPeers)
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
            // Check if message is from this peer
            if(e.RemoteIp == this.LocalIpAddress)
            {
                // From this peer.
                // no futher proccessing needed
                if(forwardAll == false)
                {
                    return;
                }
            }

            // Check if its from a new peer
            if(e.BindingType == TransportType.UDP)
            {
                string remotePeeripAddress = e.RemoteIp;
                bool peerKnown = DoesPeerExistByIp(remotePeeripAddress);
                if (!peerKnown)
                {
                    // Not a known peer
                    await DirectConnectTCPAsync(remotePeeripAddress);
                }
            }

            // Check if its a blank UDP packet
            // These are used as heart beats
            if(e.Message.Length <= 0)
            {
                return;
            }

            // Trigger sent message
            MsgReceived?.Invoke(this, e);
        }

        public void NewTCPConnection(object sender, TcpSocketListenerConnectEventArgs e)
        {
            StoreConnectedPeerTCP(e.SocketClient);
        }

        public async Task DirectConnectTCPAsync(string ipAddress)
        {
            // Send connection request
            TcpSocketClient senderTCP = new TcpSocketClient();

            // If you get an error on the line below then the person you trying to connect to
            // hasn't accepted in the incoming connection
            try
            {
                await senderTCP.ConnectAsync(LocalIpAddress ?? ipAddress, this.portNum);
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
                // Try to establish an connection with this ipAddress
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
                throw new PeerNotKnown("The player does not exist!");              
            }

            return knownPeers[peerIndex].WriteStream;
        }

        public async Task<Stream> GetReadStreamAsync(string ipAddress)
        {
            bool peerKnown = DoesPeerExistByIp(ipAddress);
            if (!peerKnown)
            {
                // Try to establish an connection with this ipAddress
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
            PeerPlayer newPeer = new PeerPlayer(socketClient);
            newPeer.MsgReceived += OnNewPeerMsgReceived;
            newPeer.peerStatusChange += OnNewPeerPeerStatusChange;
            knownPeers.Add(newPeer);

            // Tell others there is a new peer
            PeerPlayerChange?.Invoke(this, new PeerPlayerChangeEventArgs(knownPeers));
        }

        private void OnNewPeerPeerStatusChange(object sender, PeerEventArgs e)
        {
            PeerPlayer changedPeer = e.Peer;

            // Delete inactive peers
            bool isPeerActive = changedPeer.IsPeerActive;
            if(!isPeerActive)
            {
                // Delete from list
                knownPeers.Remove(changedPeer);
            }

            // Tell others a peer has been deleted
            PeerPlayerChange?.Invoke(this, new PeerPlayerChangeEventArgs(knownPeers));
        }

        private void OnNewPeerMsgReceived(object sender, MsgReceivedEventArgs e)
        {
            MsgReceived?.Invoke(this, e);
        }

        // Returns true if the ip address corresponds to known peer.
        private bool DoesPeerExistByIp(string ipAddress)
        {
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

        // Returns -1 if can't find peer
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