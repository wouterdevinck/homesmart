using Home.Core.Attributes;
using Home.Core.Configuration.Models;
using Home.Devices.Hue.Common;
using Q42.HueApi;

namespace Home.Devices.Hue.Devices {

    [Device]
    public partial class HueDimmableLightDevice : HueLightDevice {
        
        public HueDimmableLightDevice(Light light, HueClient hue, HomeConfigurationModel home) : base(light, hue, home) {}

    }

}