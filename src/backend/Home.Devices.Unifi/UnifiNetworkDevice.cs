using Home.Core.Configuration.Models;
using Home.Devices.Unifi.Models;

namespace Home.Devices.Unifi {

    public class UnifiNetworkDevice : UnifiDevice {

        public UnifiNetworkDevice(HomeConfigurationModel home, NetworkDeviceModel device) : base(home, device, $"UNIFI-NETWORK-{device.Id}") {
            Ip = device.Ip;
            UplinkMac = device.Uplink.Mac;
            UplinkPort = device.Uplink.Port;
        }

    }

}
