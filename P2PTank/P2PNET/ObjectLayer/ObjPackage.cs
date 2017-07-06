using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P2PNET.ObjectLayer
{
    public class ObjPackage<T>
    {
        public T Obj { get; set; }
        public Metadata Metadata { get; set; }
        
        public ObjPackage()
        {
        }

        //constructor
        public ObjPackage(T mObject, Metadata metadata)
        {
            this.Obj = mObject;
            this.Metadata = metadata;
        }
    }
}
