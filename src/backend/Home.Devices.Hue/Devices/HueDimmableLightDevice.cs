using System.Collections.Generic;
using System.Text.Json;
using Home.Core.Attributes;
using Home.Core.Configuration.Models;
using Home.Devices.Hue.Common;
using HueApi;
using HueApi.Models;

namespace Home.Devices.Hue.Devices {

    // TODO Remove?

    [Device]
    public partial class HueDimmableLightDevice : HueLightDevice {
        
        public HueDimmableLightDevice(Light light, Device device, ZigbeeConnectivity zigbee, LocalHueApi hue, HomeConfigurationModel home) : base(light, device, zigbee, hue, home) {}

        public new void ProcessUpdate(string type, Dictionary<string, JsonElement> data) {
            base.ProcessUpdate(type, data);
        }

    }

}