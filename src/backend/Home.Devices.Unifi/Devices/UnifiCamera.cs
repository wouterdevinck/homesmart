using Home.Core;
using Home.Core.Configuration.Models;
using Home.Devices.Unifi.Models;

namespace Home.Devices.Unifi.Devices {

    public class UnifiCamera : UnifiDevice {

        public UnifiCamera(HomeConfigurationModel home, ProtectDeviceModel device, ClientModel client) : base(home, device, $"UNIFI-PROTECT-{device.Id}") {
            Type = Helpers.GetTypeString(Helpers.DeviceType.Camera);
            Ip = client?.Ip;
            UplinkMac = device.UplinkMac;
            if (client != null) UplinkPort = client.UplinkPort;
        }

    }

}
