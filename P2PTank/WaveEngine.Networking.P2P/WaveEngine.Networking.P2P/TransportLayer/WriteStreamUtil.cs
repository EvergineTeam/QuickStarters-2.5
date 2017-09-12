using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace WaveEngine.Networking.P2P.TransportLayer
{
    class AbstractStreamUtil
    {
        public Stream ActiveStream { get; set; }

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

    class WriteStreamUtil : AbstractStreamUtil
    {
        private SemaphoreSlim queueSem = new SemaphoreSlim(1);
        private SemaphoreSlim messageSem = new SemaphoreSlim(1);

        private Queue<byte[]> msgBuffer;

        public WriteStreamUtil(Stream mWriteStream) : base(mWriteStream)
        {
            msgBuffer = new Queue<byte[]>();
        }

        public async Task WriteBytesAsync(byte[] msg)
        {
            // Add message to buffer
            await queueSem.WaitAsync();
            try
            {
                msgBuffer.Enqueue(msg);
            }
            finally
            {
                queueSem.Release();
            }

            // Make sure only one message is sent at a time
            await messageSem.WaitAsync();
            try
            {
                byte[] nextMsg;
                await queueSem.WaitAsync();
                try
                {
                    // Read next message from buffer
                    if (msgBuffer.Count <= 0)
                    {
                        throw new Exception("Expected more messages in the write buffer.");
                    }
                    nextMsg = msgBuffer.Dequeue();
                }
                finally
                {
                    queueSem.Release();
                }

                // Send number indicating message size
                int lenMsg = (int)nextMsg.Length;
                byte[] lenBin = IntToBinary(lenMsg);
                await base.ActiveStream.WriteAsync(lenBin, 0, lenBin.Length);

                // Send the msg
                if (lenMsg > 0)
                {
                    await base.ActiveStream.WriteAsync(nextMsg, 0, lenMsg);
                }
                await base.ActiveStream.FlushAsync();
            }
            finally
            {
                messageSem.Release();
            }
        }
    }
}
