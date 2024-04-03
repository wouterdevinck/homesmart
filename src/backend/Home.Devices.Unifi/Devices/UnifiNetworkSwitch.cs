using Home.Core;
using Home.Core.Configuration.Models;
using Home.Core.Devices;
using Home.Devices.Unifi.Models;

namespace Home.Devices.Unifi.Devices {

    public class UnifiNetworkSwitch : UnifiNetworkDevice, INetworkSwitch {

        public UnifiNetworkSwitch(HomeConfigurationModel home, NetworkDeviceModel device) : base(home, device) {
            Type = Helpers.GetTypeString(Helpers.DeviceType.NetworkSwitch);

        }

    }

}
