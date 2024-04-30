using System.Threading.Tasks;
using Home.Core.Attributes;
using Home.Core.Configuration.Models;
using Home.Core.Devices;
using Home.Devices.Unifi.Models;

namespace Home.Devices.Unifi.Devices {

    [Device]
    public partial class UnifiPoeNetworkSwitchDevice : UnifiNetworkSwitchDevice, IPoeNetworkSwitch {

        private readonly UnifiApiClient _api;

        public UnifiPoeNetworkSwitchDevice(HomeConfigurationModel home, NetworkDeviceModel device, UnifiApiClient api) : base(home, device) {
            _api = api;
        }

        [DeviceCommand]
        public async Task<bool> TurnPortPowerOnAsync(int port) {
            return await _api.SetSwitchPortPowerEnabledAsync(LocalId, port, true);
        }

        [DeviceCommand]
        public async Task<bool> TurnPortPowerOffAsync(int port) {
            return await _api.SetSwitchPortPowerEnabledAsync(LocalId, port, false);
        }

    }

}
