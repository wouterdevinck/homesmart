using Home.Core;
using Home.Core.Attributes;
using Home.Core.Configuration.Models;

namespace Home.Devices.Somfy.Devices {

    [Device]
    public partial class SomfyBridgeDevice : SomfyDevice {

        public SomfyBridgeDevice(HomeConfigurationModel home, string brVersion) : base(home, $"SOMFY-BRIDGE") {
            Name = "ESPSomfy RTS";
            Manufacturer = Helpers.Diy.HarmonizeManufacturer();
            Version = brVersion;
            Model =  Name;
            Type = Helpers.GetTypeString(Helpers.DeviceType.Hub);
        }

        // TODO Bridge version auto update

    }

}