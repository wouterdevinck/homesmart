using Home.Core;
using Home.Devices.Hue.Common;
using Q42.HueApi;

namespace Home.Devices.Hue.Devices {

    public partial class HueBridgeDevice : HueDevice {

        public HueBridgeDevice(Bridge bridge, HueClient hue) : base(hue, bridge.Config?.BridgeId) {
            if (bridge.Config == null) return;
            DeviceId = $"HUE-BRIDGE-{LocalId}";
            Name = bridge.Config.Name;
            Manufacturer = Helpers.Signify.HarmonizeManufacturer();
            Model = bridge.Config.ModelId;
            Version = bridge.Config.SoftwareVersion;
            Reachable = true;
            Type = Helpers.GetTypeString(Helpers.DeviceType.Hub);
        }

    }

}