using Newtonsoft.Json.Linq;
using P2PNET.ObjectLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P2PNET.ObjectLayer
{
    public class BObject
    {
        private byte[] msgBin;
        private Serializer serializer;

        //constructor
        public BObject(byte[] msg, Serializer mSerializer)
        {
            this.msgBin = msg;
            this.serializer = mSerializer;
        }

        //overwrite the GetType method
        public new string GetType()
        {
            string jsonMsg = Encoding.Unicode.GetString(msgBin, 0, msgBin.Length);
            JObject jObject = JObject.Parse(jsonMsg);
            JToken jToken = jObject["Metadata"];
            JToken jObjeType = jToken.SelectToken("ObjectType");
            return jObjeType.Value<string>();
        }

        public Metadata GetMetadata()
        {
            ObjPackage<Object> genericPackage = serializer.DeserializeObject<ObjPackage<Object>>(msgBin);
            return genericPackage.Metadata;
        }

        public T GetObject<T>()
        {
            try
            {
                ObjPackage<T> objPackage = serializer.DeserializeObject<ObjPackage<T>>(msgBin);
                return objPackage.Obj;
            }
            catch(Exception e)
            {
                throw e;
            }
        }
        

    }
}
