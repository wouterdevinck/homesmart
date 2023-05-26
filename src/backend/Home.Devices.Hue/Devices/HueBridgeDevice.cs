using Home.Core;
using Home.Core.Attributes;
using Home.Core.Configuration.Models;
using Home.Devices.Hue.Common;
using HueApi;
using HueApi.Models;

namespace Home.Devices.Hue.Devices {

    [Device]
    public partial class HueBridgeDevice : HueDevice {

        public HueBridgeDevice(Bridge bridge, Device device, LocalHueApi hue, HomeConfigurationModel home) : base(hue, bridge.Id, home, $"HUE-BRIDGE-{bridge.ExtensionData["bridge_id"]}") {
            Name = device.Metadata.Name;
            Manufacturer = device.ProductData.ManufacturerName.HarmonizeManufacturer();
            Model = device.ProductData.ModelId;
            Version = device.ProductData.SoftwareVersion;
            Reachable = true;
            Type = Helpers.GetTypeString(Helpers.DeviceType.Hub);
            HueApiId = bridge.Id;
        }

    }

}