using System.Threading.Tasks;
using Home.Core.Attributes;
using Home.Core.Configuration.Models;
using Home.Core.Devices;
using Home.Devices.Unifi.Models;

namespace Home.Devices.Unifi.Devices {

    [Device]
    public partial class UnifiPoeNetworkSwitch : UnifiNetworkSwitch, IPoeNetworkSwitch {

        private readonly string _id;
        private readonly UnifiApiClient _api;

        public UnifiPoeNetworkSwitch(HomeConfigurationModel home, NetworkDeviceModel device, UnifiApiClient api) : base(home, device) {
            _id = device.Id;
            _api = api;
        }

        [DeviceCommand]
        public async Task TurnPortPowerOnAsync(int port) {
            await _api.SetSwitchPortPowerEnabledAsync(_id, port, true);
            // TODO Error check
            // TODO Notify change
        }

        [DeviceCommand]
        public async Task TurnPortPowerOffAsync(int port) {
            await _api.SetSwitchPortPowerEnabledAsync(_id, port, false); 
            // TODO Error check
            // TODO Notify change
        }

    }

}
