using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P2PNET.TransportLayer
{
    class AbstractStreamUtil
    {
        public Stream ActiveStream { get; set; }

        //constructor
        public AbstractStreamUtil(Stream inputStream)
        {
            this.ActiveStream = inputStream;
        }

        protected byte[] IntToBinary(int value)
        {
            byte[] valueBin = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(valueBin);
            }
            return valueBin;
        }

        protected int BinaryToInt(byte[] binArray)
        {
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(binArray);
            }
            return BitConverter.ToInt32(binArray, 0);
        }
    }
}
