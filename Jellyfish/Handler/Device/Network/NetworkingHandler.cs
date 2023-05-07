using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JellyFish.Handler.Abstraction;
using Perms = Microsoft.Maui.ApplicationModel;

namespace JellyFish.Handler.Device.Network
{
    public class NetworkingHandler : AbstractDeviceActionHandler<Permissions.NetworkState>
    {
        public bool IsInternetConnectionGiven()
        {
            var networkAccess = Connectivity.Current.NetworkAccess;
            return networkAccess == NetworkAccess.Internet;
        }
        public NetworkAccess GetCurrentNetworkAccess()
        {
            return Connectivity.Current.NetworkAccess;
        }
    }
}
