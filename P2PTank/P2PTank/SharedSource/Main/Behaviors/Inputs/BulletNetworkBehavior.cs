using System;
using System.Collections.Generic;
using System.Text;
using P2PTank.Managers;
using WaveEngine.Framework;

namespace P2PTank.Behaviors
{
    public class BulletNetworkBehavior : Behavior
    {
        public string BulletID { get; set; }

        public BulletNetworkBehavior(P2PManager p2pManager = null)
        {
        }

        protected override void Update(TimeSpan gameTime)
        {
        }
    }
}
