using Home.Core;
using Home.Core.Attributes;
using Home.Core.Configuration.Models;
using Home.Devices.Hue.Common;
using Q42.HueApi;

namespace Home.Devices.Hue.Devices {

    [Device]
    public partial class HueBridgeDevice : HueDevice {

        public HueBridgeDevice(Bridge bridge, HueClient hue, HomeConfigurationModel home) : base(hue, bridge.Config?.BridgeId, home, $"HUE-BRIDGE-{bridge.Config?.BridgeId}") {
            if (bridge.Config == null) return;
            Name = bridge.Config.Name;
            Manufacturer = Helpers.Signify.HarmonizeManufacturer();
            Model = bridge.Config.ModelId;
            Version = bridge.Config.SoftwareVersion;
            Reachable = true;
            Type = Helpers.GetTypeString(Helpers.DeviceType.Hub);
        }

    }

}