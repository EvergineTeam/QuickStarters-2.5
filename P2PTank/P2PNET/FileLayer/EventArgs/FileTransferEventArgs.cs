using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P2PNET.FileLayer.EventArgs
{
    public enum TransDirrection
    {
        sending,
        receiving
    }
    public class FileTransferEventArgs : System.EventArgs
    {
        public TransDirrection Dirrection { get; }
        public string FileName { get; }
        public long FileLength { get; }
        public long BytesProcessed { get; }
        public float Percent
        {
            get
            {
                return (float)BytesProcessed / FileLength;
            }
        }

        //constructor
        public FileTransferEventArgs(FileTransReq fileTrans, TransDirrection mDir)
        {
            this.FileLength = fileTrans.FileDetails.FileSize;
            this.BytesProcessed = fileTrans.BytesProcessed;
            this.FileName = fileTrans.FileDetails.FileName;
            this.Dirrection = mDir;
        }
    }
}
