using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P2PNET.TransportLayer
{
    class ReadStreamUtil : AbstractStreamUtil
    {
        //constructor
        public ReadStreamUtil(Stream mReadStream) : base(mReadStream)
        {

        }

        public async Task<byte[]> ReadBytesAsync()
        {
            //read the first 4 bytes = sizeof(int)
            const int intSize = sizeof(int);
            Byte[] lengthBin = await ReadBytesAsync(intSize);
            int msgSize = BinaryToInt(lengthBin);

            //read message
            byte[] messageBin = await ReadBytesAsync(msgSize);
            return messageBin;
        }

        private async Task<byte[]> ReadBytesAsync(int bytesToRead)
        {
            Byte[] msgBin = new Byte[bytesToRead];
            int totalBytesRd = 0;
            while (totalBytesRd < bytesToRead)
            {
                //ReadAsync() can return less then intSize therefore keep on looping until intSize is reached
                byte[] tempMsgBin = new Byte[bytesToRead];
                int bytesRead = await base.ActiveStream.ReadAsync(tempMsgBin, 0, bytesToRead - totalBytesRd);
                Array.Copy(tempMsgBin, 0, msgBin, totalBytesRd, bytesRead);
                totalBytesRd += bytesRead;
            }
            return msgBin;
        }

    }
}
