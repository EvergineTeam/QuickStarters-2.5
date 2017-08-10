using P2PNET.TransportLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace P2PNET.ObjectLayer
{
    public class Metadata
    {
        //the type of the object
        public string ObjectType { get; set; }

        public string SourceIp { get; set; }

        //UDP or TCP
        public TransportType? BindType { get; set; }
    }
}
