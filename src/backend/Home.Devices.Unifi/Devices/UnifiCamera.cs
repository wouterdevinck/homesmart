using System;
using System.Linq;
using System.Threading.Tasks;
using Home.Core;
using Home.Core.Attributes;
using Home.Core.Configuration.Models;
using Home.Core.Devices;
using Home.Core.Relationships;
using Home.Devices.Unifi.Models;

namespace Home.Devices.Unifi.Devices {

    [Device]
    public partial class UnifiCamera : UnifiDevice, ICamera {

        [DeviceProperty] 
        public bool On { get; set; } // TODO Get switch port state

        public UnifiCamera(HomeConfigurationModel home, ProtectDeviceModel device, ClientModel client) : base(home, device, $"UNIFI-PROTECT-{device.Id}") {
            Type = Helpers.GetTypeString(Helpers.DeviceType.Camera);
            Ip = client?.Ip;
            UplinkMac = device.UplinkMac;
            if (client != null) UplinkPort = client.UplinkPort;
        }

        [DeviceCommand]
        public Task ToggleOnOffAsync() {
            return On ? TurnOffAsync() : TurnOnAsync();
        }

        [DeviceCommand]
        public async Task TurnOnAsync() {
            if (RelatedDevices.SingleOrDefault(x => x is ParentNetworkSwitch && x.Device is IPoeNetworkSwitch) is ParentNetworkSwitch { Device: IPoeNetworkSwitch device, SwitchPort: var port }) {
                await device.TurnPortPowerOnAsync(port);
            } else {
                throw new NotImplementedException();
            }
            // TODO Notify change
        }

        [DeviceCommand]
        public async Task TurnOffAsync() {
            if (RelatedDevices.SingleOrDefault(x => x is ParentNetworkSwitch && x.Device is IPoeNetworkSwitch) is ParentNetworkSwitch { Device: IPoeNetworkSwitch device, SwitchPort: var port }) {
                await device.TurnPortPowerOffAsync(port);
            } else {
                throw new NotImplementedException();
            }
            // TODO Notify change
        }

    }

}
