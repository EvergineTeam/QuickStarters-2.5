using P2PNET.TransportLayer;
using System.Diagnostics;
using System.Text;

namespace WaveEngine.Networking.P2P
{
    public class HeartBeatManager
    {
        private string heartBeatMsg;
        private TransportManager transMgr;
        private Timer hrtBtTimer;

        public HeartBeatManager(string mHeartBeatMsg, TransportManager mTransMgr)
        {
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