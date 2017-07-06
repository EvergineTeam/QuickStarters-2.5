using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P2PNET.FileLayer.SendableObjects
{
    public class FileReqMeta
    {
        //data the receiver can use to decide whether or not to reject the request
        public List<FileMeta> Files { get; set; }
        public int BufferSize { get; set; }

        //identification data
        public string SenderIpAddress { get; set; }

        public FileReqMeta(List<FileMeta> mFiles, int mBufferSize, string mSenderIpAddress)
        {
            this.Files = mFiles;
            this.BufferSize = mBufferSize;
            this.SenderIpAddress = mSenderIpAddress;
        }
    }
}
