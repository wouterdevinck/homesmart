using Home.Devices.Hue.Common;
using Q42.HueApi;

namespace Home.Devices.Hue.Devices {

    public partial class HueDimmableLightDevice : HueLightDevice {
        
        public HueDimmableLightDevice(Light light, HueClient hue) : base(light, hue) {}

    }

}