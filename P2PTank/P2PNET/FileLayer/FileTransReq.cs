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
    public class FileTransReq
    {
        //file details
        public FileMeta FileDetails { get; }

        public int curFilePartNum
        {
            get
            {
                return (int)Math.Ceiling((float)BytesProcessed / bufferSize);
            }
        }
        public int TotalPartNum
        {
            get
            {
                return (int)Math.Ceiling((float)FileDetails.FileSize / bufferSize);
            }
        }

        //transmittion details
        public long BytesProcessed
        {
            get
            {
                return bytesProccessed;
            }
        }
        private long bytesProccessed;

        //buffer size is only for file parts calculations 
        private int bufferSize;

        private Stream fileDataStream;

        //constructor
        public FileTransReq(IFile mFileDetails, Stream mFileStream, int mBufferSize)
        {
            this.FileDetails = new FileMeta(mFileDetails.Name, mFileDetails.Path, mFileStream.Length);
            this.bufferSize = mBufferSize;
            this.bytesProccessed = 0;
            this.fileDataStream = mFileStream;
        }

        //constructor
        public FileTransReq(FileMeta fileDetails, Stream mFileStream, int mBufferSize)
        {
            this.FileDetails = new FileMeta(fileDetails.FileName, fileDetails.FilePath, fileDetails.FileSize);
            this.bufferSize = mBufferSize;
            this.bytesProccessed = 0;
            this.fileDataStream = mFileStream;
        }

        //deconstructor

        public async Task<byte[]> ReadBytes(int numOfBytes)
        {
            byte[] fileData = new byte[numOfBytes];

            await this.fileDataStream.ReadAsync(fileData, 0, numOfBytes);

            this.bytesProccessed += numOfBytes;

            return fileData;
        }

        public async Task WriteBytes(byte[] data)
        {
            await this.fileDataStream.WriteAsync(data, 0, data.Length);
            this.bytesProccessed += data.Length;
        }

        public async Task CloseFileStream()
        {
            await this.fileDataStream.FlushAsync();
            this.fileDataStream.Dispose();
        }
    }
}
