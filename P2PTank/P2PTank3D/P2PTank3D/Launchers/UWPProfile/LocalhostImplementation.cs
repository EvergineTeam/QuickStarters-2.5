using P2PTank3D.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Connectivity;

namespace P2PTank3D
{
    public class LocalhostImplementation : ILocalhost
    {
        public string Ip
        {
            get
            {
                var icp = NetworkInformation.GetInternetConnectionProfile();

                if (icp?.NetworkAdapter == null) return null;
                var hostname =
                    NetworkInformation.GetHostNames()
                        .FirstOrDefault(
                            hn =>
                                hn.Type == HostNameType.Ipv4 &&
                                hn.IPInformation?.NetworkAdapter != null &&
                                hn.IPInformation.NetworkAdapter.NetworkAdapterId == icp.NetworkAdapter.NetworkAdapterId);

                // the ip address
                return hostname?.CanonicalName;
            }
        }
    }
}
