using Home.Core.Attributes;
using Home.Core.Configuration.Models;
using Home.Devices.Unifi.Models;

namespace Home.Devices.Unifi {

    [Device]
    public partial class UnifiNetworkDevice : UnifiDevice {

        public UnifiNetworkDevice(HomeConfigurationModel home, NetworkDeviceModel device) : base(home, device, $"UNIFI-NETWORK-{device.Id}") {
            Ip = device.Ip;
            UplinkMac = device.Uplink.Mac;
            UplinkPort = device.Uplink.Port;
        }

        public void ProcessUpdate(NetworkDeviceModel update) {
            if (Ip != update.Ip) {
                Ip = update.Ip;
                NotifyObservers(nameof(Ip), Ip);
            }
            if (UplinkMac != update.Uplink.Mac) {
                UplinkMac = update.Uplink.Mac;
                NotifyObservers(nameof(UplinkMac), UplinkMac);
            }
            if (UplinkPort != update.Uplink.Port) {
                UplinkPort = update.Uplink.Port;
                NotifyObservers(nameof(UplinkPort), UplinkPort);
            }
            base.ProcessUpdate(update);
        }

    }

}
