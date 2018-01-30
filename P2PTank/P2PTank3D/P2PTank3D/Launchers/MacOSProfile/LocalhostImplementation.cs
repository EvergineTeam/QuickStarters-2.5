using P2PTank3D.Services;
using System.Net;

namespace P2PTank3D
{
    public class LocalhostImplementation : ILocalhost
    {
        public string Ip
        {
            get
            {
                string hostName = Dns.GetHostName(); // Retrive the Name of HOST  
                string ip = Dns.GetHostEntry(hostName).AddressList[0].ToString();

                return ip;
            }
        }
    }
}