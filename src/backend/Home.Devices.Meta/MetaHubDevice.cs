using System.Threading.Tasks;
using Home.Core;
using Home.Core.Attributes;
using Home.Core.Configuration.Models;
using Home.Core.Interfaces;
using Home.Devices.Meta.Models;

namespace Home.Devices.Meta {

    [Device]
    public partial class MetaHubDevice : AbstractDevice, IDevice {

        [DeviceProperty]
        public string OsBootSlot { get; private set; }

        [DeviceProperty]
        public string OsVersion { get; private set; }

        private readonly EdgeOsApiClient _api;

        public MetaHubDevice(HomeConfigurationModel home, EdgeOsApiClient api, EdgeOsStatusResponseModel status) : base(home, "META-HUB") {
            _api = api;
            Name = status.AppName;
            Manufacturer = status.AppName;
            Version = status.AppVersion;
            Model = status.AppName;
            Type = Helpers.GetTypeString(Helpers.DeviceType.Hub);
            Reachable = true;
            OsBootSlot = status.BootedFrom;
            OsVersion = status.EdgeOsVersion;
        }

        [DeviceCommand]
        public async Task RebootAsync() {
            await _api.Reboot();
        }

        [DeviceCommand]
        public async Task SwitchBootAsync() {
            await _api.SwitchBoot();   // TODO re-read status? reboot?
        }

        [DeviceCommand]
        public async Task UpgradeAsync(string url) {
            await _api.Upgrade(url);   // TODO status tracking? re-read status?
            // Failing on wget not supporting https
        }

    }

}