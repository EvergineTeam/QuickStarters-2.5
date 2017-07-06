using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P2PNET.FileLayer.SendableObjects
{
    class ReqAck
    {
        public bool AcceptedFile { get; set; }

        //constructor
        public ReqAck(bool mAcceptedFile)
        {
            this.AcceptedFile = mAcceptedFile;
        }
    }
}
