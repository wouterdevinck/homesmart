using Home.Core;
using Home.Core.Attributes;
using Home.Core.Configuration.Models;
using Home.Core.Devices;
using Home.Devices.Unifi.Models;

namespace Home.Devices.Unifi.Devices {

    [Device]
    public partial class UnifiConsole : UnifiNetworkDevice, INetworkSwitch {

        public UnifiConsole(HomeConfigurationModel home, NetworkDeviceModel device) : base(home, device) {
            Type = Helpers.GetTypeString(Helpers.DeviceType.Hub);

        }

    }

}
