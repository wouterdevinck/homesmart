using Home.Core;
using Home.Core.Configuration.Models;
using Home.Devices.Unifi.Models;

namespace Home.Devices.Unifi.Devices {

    public class UnifiConsole : UnifiNetworkDevice {

        public UnifiConsole(HomeConfigurationModel home, NetworkDeviceModel device) : base(home, device) {
            Type = Helpers.GetTypeString(Helpers.DeviceType.Hub);

        }

    }

}
