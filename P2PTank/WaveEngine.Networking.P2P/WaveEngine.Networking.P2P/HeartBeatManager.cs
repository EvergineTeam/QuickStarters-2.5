using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WaveEngine.Networking.P2P.TransportLayer;

namespace WaveEngine.Networking.P2P
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
                    Task.Run(() => tuple.Item1(tuple.Item2));
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
        private string heartBeatMsg;
        private TransportManager transMgr;
        private Timer hrtBtTimer;

        public HeartBeatManager(string mHeartBeatMsg, TransportManager mTransMgr)
        {
            // Periodic signal 
            this.heartBeatMsg = mHeartBeatMsg;
            this.transMgr = mTransMgr;
        }

        public void StartBroadcasting()
        {
            this.hrtBtTimer = new Timer(TimerCallBack, null, 0, 1000);
        }

        public void EndBroadcasting()
        {
            this.hrtBtTimer.Cancel();
        }
        
        private async void TimerCallBack(object parameter)
        {
            byte[] msgBin = Encoding.UTF8.GetBytes(heartBeatMsg);
            await this.transMgr.SendBroadcastAsyncUDP(msgBin);
            Debug.WriteLine("Sent heartbeat");
        }
    }
}