using P2PNET.FileLayer.EventArgs;
using P2PNET.FileLayer.SendableObjects;
using P2PNET.ObjectLayer;
using PCLStorage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P2PNET.FileLayer
{
    public class FileSentReq
    {
        //for identification FileSentReq
        public string targetIpAddress { get; }

        public List<FileTransReq> FileTransReqs { get; }
        private int bufferSize;

        //constructor
        public FileSentReq(List<FileTransReq> mFileTransReqs, int mBufferSize, string mIpAddress)
        {
            this.bufferSize = mBufferSize;
            this.FileTransReqs = mFileTransReqs;
            this.targetIpAddress = mIpAddress;
        }
        

        public FileReqMeta GenerateMetadataRequest()
        {
            //collect details from all fileTransRequests
            List<FileMeta> fileDetails = new List<FileMeta>();
            foreach(FileTransReq fileTransReq in FileTransReqs)
            {
                fileDetails.Add(fileTransReq.FileDetails);
            }
            FileReqMeta fileSendMetadata = new FileReqMeta(fileDetails, bufferSize, targetIpAddress);
            return fileSendMetadata;
        }

        public bool FileHasMoreParts(FileTransReq fileTrans)
        {
            if(fileTrans.curFilePartNum < fileTrans.TotalPartNum)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<FilePartObj> GetNextFilePart(FileTransReq mCurFileTrans)
        {
            //set buffer lengths
            int bufferLen = (int)Math.Min(mCurFileTrans.FileDetails.FileSize - mCurFileTrans.BytesProcessed, this.bufferSize);
            if(bufferLen <= 0)
            {
                //nothing more to be sent
                return null;
            }
            FileMeta fileMetadata = mCurFileTrans.FileDetails;
            byte[] fileData = await mCurFileTrans.ReadBytes(bufferLen);


            //populate File part object
            FilePartObj filePart = new FilePartObj(fileMetadata, fileData, mCurFileTrans.curFilePartNum, mCurFileTrans.TotalPartNum);

            return filePart;
        }
    }
}
