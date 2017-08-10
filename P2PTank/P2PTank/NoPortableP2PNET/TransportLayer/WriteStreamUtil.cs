using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace P2PNET.TransportLayer
{
    class WriteStreamUtil : AbstractStreamUtil
    {
        private SemaphoreSlim queueSem = new SemaphoreSlim(1);
        private SemaphoreSlim messageSem = new SemaphoreSlim(1);

        private Queue<byte[]> msgBuffer;

        //constructor
        public WriteStreamUtil(Stream mWriteStream ) : base(mWriteStream)
        {
            msgBuffer = new Queue<byte[]>();
        }

        public async Task WriteBytesAsync(byte[] msg)
        {
            //add message to buffer
            await queueSem.WaitAsync();
            try
            {
                msgBuffer.Enqueue(msg);
            }
            finally
            {
                queueSem.Release();
            }

            //make sure only one message is sent at a time
            await messageSem.WaitAsync();
            try
            {
                byte[] nextMsg;
                await queueSem.WaitAsync();
                try
                {
                    //read next message from buffer
                    if (msgBuffer.Count <= 0)
                    {
                        throw new LowLevelTransitionError("Expected more messages in the write buffer.");
                    }
                    nextMsg = msgBuffer.Dequeue();
                }
                finally
                {
                    queueSem.Release();
                }

                //send number indicating message size
                int lenMsg = (int)nextMsg.Length;
                byte[] lenBin = IntToBinary(lenMsg);
                await base.ActiveStream.WriteAsync(lenBin, 0, lenBin.Length);

                //send the msg
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
