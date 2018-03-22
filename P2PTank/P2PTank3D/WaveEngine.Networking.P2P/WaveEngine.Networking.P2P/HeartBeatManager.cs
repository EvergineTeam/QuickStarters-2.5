using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Networking.P2P.TransportLayer;

namespace Networking.P2P
{
    internal delegate void TimerCallback(object state);

    internal sealed class Timer : CancellationTokenSource, IDisposable
    {
        public Timer(TimerCallback callback, object state, int dueTime, int period)
        {
            Task.Delay(dueTime, Token).ContinueWith(async (t, s) =>
            {
                var tuple = (Tuple<TimerCallback, object>)s;

                while (true)
                {
                    if (IsCancellationRequested)
                        break;
                    await Task.Run(() => tuple.Item1(tuple.Item2));
                    await Task.Delay(period);
                }

            }, Tuple.Create(callback, state), CancellationToken.None,
                TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion,
                TaskScheduler.Default);
        }

        public new void Dispose() { base.Cancel(); }
    }

    internal class HeartBeatManager
    {
        private TransportManager transMgr;
        private Timer hrtBtTimer;
        
        public string HeartBeatMessage { get; set; }

        public HeartBeatManager(TransportManager mTransMgr)
        {
            // Periodic signal 
            this.transMgr = mTransMgr;
        }

        public void StartBroadcasting(int milliSeconds = 1000)
        {
            this.hrtBtTimer = new Timer(TimerCallBack, null, 0, milliSeconds);
        }

        public void EndBroadcasting()
        {
            this.hrtBtTimer.Cancel();
        }
        
        private async void TimerCallBack(object parameter)
        {
            byte[] msgBin = Encoding.UTF8.GetBytes(HeartBeatMessage);
            await this.transMgr.SendBroadcastAsyncUDP(msgBin);
            Debug.WriteLine("Sent heartbeat");
        }
    }
}