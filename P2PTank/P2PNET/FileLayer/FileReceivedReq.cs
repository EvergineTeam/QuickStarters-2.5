using P2PNET.FileLayer.SendableObjects;
using PCLStorage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P2PNET.FileLayer
{
    public class FileReceiveReq
    {
        //for identification FileReceiveReq
        public string SenderIpAddress { get; }

        private List<FileTransReq> fileTransReqs;


        //constructor
        public FileReceiveReq(List<FileTransReq> mFileTransReqs, string mIpAddress)
        {
            this.fileTransReqs = mFileTransReqs;
            this.SenderIpAddress = mIpAddress;
        }


        public async Task WriteFilePartToFile(FilePartObj receivedFilePart)
        {
            FileTransReq fileTrans = GetFileTransReqFromFileMeta(receivedFilePart.FileMetadata);

            //validate packet recieved
            bool isValid = ValidFilePart(receivedFilePart);
            if(!isValid)
            {
                throw new FileTransitionError("file part received is not valided for this peer.");
            }

            //valid file part
            byte[] buffer = receivedFilePart.FileData;
            await fileTrans.WriteBytes(buffer);
        }

        public FileTransReq GetFileTransReqFromFileMeta(FileMeta mFileMeta)
        {
            //collect info to identify file transfer object
            string fileName = mFileMeta.FileName;
            string filePath = mFileMeta.FilePath;
            foreach(FileTransReq fileTrans in fileTransReqs)
            {
                if(fileTrans.FileDetails.FileName == fileName && fileTrans.FileDetails.FilePath == filePath)
                {
                    return fileTrans;
                }

            }

            //can't find file transfer object
            return null;
        }

        private bool ValidFilePart(FilePartObj filePart)
        {
            if(filePart == null)
            {
                return false;
            }

            //TODO
            return true;
        }

        public async Task CloseRequest()
        {
            foreach (FileTransReq fileTrans in fileTransReqs)
            {
                await fileTrans.CloseFileStream();
            }
        }
    }
}
