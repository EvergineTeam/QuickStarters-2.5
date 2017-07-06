#if FILE
using P2PNET.FileLayer.EventArgs;
using P2PNET.FileLayer.SendableObjects;
using P2PNET.ObjectLayer;
using P2PNET.ObjectLayer.EventArgs;
using P2PNET.TransportLayer;
using P2PNET.TransportLayer.EventArgs;
using PCLStorage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P2PNET.FileLayer
{
    /// <summary>
    /// Class for sending and receiving files between peers.
    /// Built on top of <C>ObjectManager</C>
    /// </summary>
    public class FileManager
    {
        /// <summary>
        /// triggered when a file part has been sent or received sucessfully.
        /// </summary>
        public event EventHandler<FileTransferEventArgs> FileProgUpdate;

        /// <summary>
        /// Triggered when a message containing an object has been received
        /// </summary>
        public event EventHandler<ObjReceivedEventArgs> ObjReceived;

        /// <summary>
        /// Triggered when a new peer is detected or an existing peer becomes inactive
        /// </summary>
        public event EventHandler<PeerChangeEventArgs> PeerChange;

        /// <summary>
        /// Triggered when a whole file have been received from another peer
        /// </summary>
        public event EventHandler<FileReceivedEventArgs> FileReceived;

        /// <summary>
        /// list of peers discovered by detecting their network activities
        /// </summary>
        public List<Peer> KnownPeers
        {
            get
            {
                return ObjectManager.KnownPeers;
            }
        }

        /// <summary>
        /// the default path to store incoming files
        /// </summary>
        public string DefaultFilePath { get; set; }

        /// <summary>
        /// A reference to the object manager used to pass object messages between peers
        /// </summary>
        public ObjectManager ObjectManager { get; set; }


        private IFileSystem fileSystem;
        private List<FileReceiveReq> receivedFileRequests;
        private List<FileSentReq> sendFileRequests;
        private TaskCompletionSource<bool> stillProcPrevMsg;

        /// <summary>
        /// Constructor that instantiates a file manager. To commence listening call the method <C>StartAsync</C>.
        /// </summary>
        /// <param name="mPortNum"> The port number which this peer will listen on and send messages with </param>
        /// <param name="mForwardAll"> When true, all messages received trigger a MsgReceived event. This includes UDB broadcasts that are reflected back to the local peer.</param>
        /// <param name="defaultFilePath">the root path to store incoming files</param>
        public FileManager(int portNum = 8080, string defaultFilePath = "./temp/", bool mForwardAll = false, ILogger mLogger = null)
        {
            this.receivedFileRequests = new List<FileReceiveReq>();
            this.sendFileRequests = new List<FileSentReq>();
            this.stillProcPrevMsg = new TaskCompletionSource<bool>();
            this.ObjectManager = new ObjectManager(portNum, mForwardAll, false, mLogger);
            this.fileSystem = FileSystem.Current;
            this.DefaultFilePath = defaultFilePath;

            this.ObjectManager.ObjReceived += ObjManager_objReceived;
            this.ObjectManager.PeerChange += ObjManager_PeerChange;
        }

        /// <summary>
        /// Constructor that instantiates a file manager. To commence listening call the method <C>StartAsync</C>.
        /// </summary>
        /// <param name="mObjectManager"> A refernece to an existing ObjectManager </param>
        /// <param name="defaultFilePath">the root path to store incoming files</param>
        public FileManager(ObjectManager mObjectManager, string defaultFilePath = "./temp/")
        {
            this.receivedFileRequests = new List<FileReceiveReq>();
            this.sendFileRequests = new List<FileSentReq>();
            this.stillProcPrevMsg = new TaskCompletionSource<bool>();
            this.ObjectManager = mObjectManager;
            this.fileSystem = FileSystem.Current;
            this.DefaultFilePath = defaultFilePath;

            this.ObjectManager.ObjReceived += ObjManager_objReceived;
            this.ObjectManager.PeerChange += ObjManager_PeerChange;
        }

        /// <summary>
        /// Peer will start actively listening on the specified port number.
        /// </summary>
        /// <returns></returns>
        public async Task StartAsync()
        {
            await ObjectManager.StartAsync();
        }

        //TODO: update description
        /// <summary>
        /// send file to the peer with the given IP address via a reliable TCP connection. 
        /// Works by breaking down the file into blocks each of length <C>bufferSize</C>. Each block is
        /// then compressed and sent one by one to the other peer. 
        /// 
        /// Note: can only send one file request at a time. Wait until This function has triggered an
        /// file received event before continuing
        /// </summary>
        /// <param name="ipAddress">The IP address of the peer to send the message to</param>
        /// <param name="filePath">The path to the file you want to send</param>
        /// <param name="bufferSize">
        /// Using a small buffer size will trigger <c>FileProgUpdate</c>> more but
        /// will also increase buffer overhead. Buffer size is also the max amount of memory
        /// a file will occupy in RAM.
        /// </param>
        /// <returns></returns>
        public async Task SendFileAsync(string ipAddress, List<string> filePaths, int bufferSize = 100 * 1024)
        {
            //TODO: check if there is an active sendFile request to the target ip address

            //create a file send request
            FileSentReq fileSentRequest = await CreateFileSendReq(ipAddress, filePaths, bufferSize);
            sendFileRequests.Add(fileSentRequest);

            //send file request metadata to receiver
            FileReqMeta fileMetadata = fileSentRequest.GenerateMetadataRequest();
            
            await ObjectManager.SendAsyncTCP(ipAddress, fileMetadata);

            //receiver will then send back a acceptance message which is proccessed in ProcessAckMessage()
        }

        private void ObjManager_PeerChange(object sender, PeerChangeEventArgs e)
        {
            this.PeerChange?.Invoke(this, e);
        }


        private async Task<FileSentReq> CreateFileSendReq(string ipAddress, List<string> filePaths, int bufferSize)
        {
            List<FileTransReq> fileTransReqs = new List<FileTransReq>();

            //create a fileTrans for each file
            foreach (string filePath in filePaths)
            {
                //TODO: handle folders as well as files
                //get file details
                IFile file = await fileSystem.GetFileFromPathAsync(filePath);
                //TODO: check if file is already open
                Stream fileStream;
                try
                {
                    fileStream = await file.OpenAsync(FileAccess.Read);
                }
                catch
                {
                    //can't find file
                    throw new FileNotFound("Can't access the file: " + filePath);
                }

                //store away file details
                FileTransReq fileTransReq = new FileTransReq(file, fileStream, bufferSize);
                fileTransReqs.Add(fileTransReq);
            }
            FileSentReq fileSendReq = new FileSentReq(fileTransReqs, bufferSize, ipAddress);
            return fileSendReq;
        }

        private async void ObjManager_objReceived(object sender, ObjReceivedEventArgs e)
        {
            
            BObject bObj = e.Obj;
            Metadata metadata = bObj.GetMetadata();
            ObjReceived?.Invoke(this, e);

            string objType = bObj.GetType();
            switch (objType)
            {
                case "FilePartObj":
                    FilePartObj filePart = e.Obj.GetObject<FilePartObj>();
                    await ProcessFilePart(filePart, metadata);
                    break;
                    /*
                case "FileAck":
                    FileAck ackMsg = e.Obj.GetObject<FileAck>();
                    await ProcessAckMessage(ackMsg, metadata);
                    break;
                    */
                case "FileReqMeta":
                    FileReqMeta fileRequestMetadata = e.Obj.GetObject<FileReqMeta>();
                    await ProcessIncomingFilesRequest(fileRequestMetadata, metadata);
                    break;
                    
                case "ReqAck":
                    ReqAck fileReqAck = e.Obj.GetObject<ReqAck>();
                    await ProcessFileReqAck(fileReqAck, metadata);
                    break;
                    
                default:
                    break;
            } 
        }

        private async Task ProcessIncomingFilesRequest(FileReqMeta fileRequest, Metadata mMetadata)
        {
            //determine whether or not to accept the incoming file request
            bool validFileRequest = IsValidFileRequest(fileRequest, mMetadata);

            FileReceiveReq fileReceiveReq = null;
            if (validFileRequest)
            {
                //add to a file recieved request
                fileReceiveReq = await CreateFileReceiveReq(fileRequest, mMetadata);
                receivedFileRequests.Add(fileReceiveReq);
            }

            //send message back to sender
            ReqAck fileRequestResp = new ReqAck(validFileRequest);
            await ObjectManager.SendAsyncTCP(mMetadata.SourceIp, fileRequestResp);
        }

        private async Task<FileReceiveReq> CreateFileReceiveReq(FileReqMeta fileRequest, Metadata mMetadata)
        {
            //open stream for all files being received
            List<FileTransReq> fileTrans = new List<FileTransReq>();
            foreach(FileMeta file in fileRequest.Files)
            {
                FileTransReq fileTran = await SetupTransmitionForNewFile(file, fileRequest.BufferSize);
                fileTrans.Add(fileTran);
            }

            FileReceiveReq fileReceivedReq = new FileReceiveReq(fileTrans, mMetadata.SourceIp);
            return fileReceivedReq;
        }

        
        private async Task ProcessFileReqAck(ReqAck mFileReqAck, Metadata mMetadata)
        {
            //see if received accepted the file transmition request
            bool acceptedReq = mFileReqAck.AcceptedFile;
            FileSentReq fileSendReq = GetSendFileReqFromMeta(mMetadata);
            if (!acceptedReq)
            {
                //didn't accept request
                sendFileRequests.Remove(fileSendReq);
                return;
            }

            //start sending the file parts
            foreach(FileTransReq fileTrans in fileSendReq.FileTransReqs)
            {
                //for each file
                while (fileSendReq.FileHasMoreParts(fileTrans))
                {
                    //send all its parts
                    FilePartObj filePart = await fileSendReq.GetNextFilePart(fileTrans);
                    await ObjectManager.SendAsyncTCP(fileSendReq.targetIpAddress, filePart);

                    //send update event
                    FileProgUpdate?.Invoke(this, new FileTransferEventArgs(fileTrans, TransDirrection.sending));
                }
            }
        }
        

        private bool IsValidFileRequest(FileReqMeta fileRequest, Metadata metadata)
        {
            //TODO: check file metadata and then reject/Accept request
            //TODO: also reject request if there is already an activity request from the same peer
            return true;
        }

        //called when a file part is received
        private async Task ProcessFilePart(FilePartObj filePart, Metadata metadata)
        {
            //check if file part is valid
            if (filePart == null)
            {
                throw new Exception("filePart has not been set.");
            }

            //find file recieve request
            FileReceiveReq fileReceived = GetReceivedFileReqFromMeta(metadata);
            FileTransReq fileTrans = fileReceived.GetFileTransReqFromFileMeta(filePart.FileMetadata);

            //dump file data to disk
            await fileReceived.WriteFilePartToFile(filePart);

            //log incoming file
            FileProgUpdate?.Invoke(this, new FileTransferEventArgs(fileTrans, TransDirrection.receiving));
        }
        
        private async Task<FileTransReq> SetupTransmitionForNewFile(FileMeta fileDetails, int bufferSize)
        {
            //create a folder to store the file
            IFolder root = await fileSystem.GetFolderFromPathAsync("./");
            if (await root.CheckExistsAsync(DefaultFilePath) == ExistenceCheckResult.NotFound)
            {
                //create folder
                await root.CreateFolderAsync(DefaultFilePath, CreationCollisionOption.FailIfExists);
            }
            IFolder tempFolder = await fileSystem.GetFolderFromPathAsync(DefaultFilePath);

            //create empty file stream
            IFile newFile = await tempFolder.CreateFileAsync(fileDetails.FileName, CreationCollisionOption.ReplaceExisting);
            Stream fileStream = await newFile.OpenAsync(FileAccess.ReadAndWrite);

            //return file trans. object
            FileTransReq fileTrans = new FileTransReq(fileDetails, fileStream, bufferSize);
            return fileTrans;
        }

        private FileReceiveReq GetReceivedFileReqFromMeta( Metadata metadata )
        {
            foreach (FileReceiveReq receivedFile in this.receivedFileRequests)
            {
                if (receivedFile.SenderIpAddress == metadata.SourceIp)
                {
                    return receivedFile;
                }
            }
            //can't find coresponding file
            throw new FileNotFound("Recieved an file part but can't find file in received storage.");
        }

        //find a match based on remote ip, file name and file path
        private FileSentReq GetSendFileReqFromMeta(Metadata metadata)
        {
            //find corresponding sentFiles
            foreach (FileSentReq fileSent in sendFileRequests)
            {
                if (fileSent.targetIpAddress == metadata.SourceIp)
                {
                    return fileSent;
                }
            }
            //can't find coresponding file
            throw new FileNotFound("Recieved an Ack but can't find file in sent storage.");
        }

    }
}

#endif