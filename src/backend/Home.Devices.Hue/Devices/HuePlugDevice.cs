using Home.Core;
using Home.Core.Attributes;
using Home.Devices.Hue.Common;
using Q42.HueApi;

namespace Home.Devices.Hue.Devices {

    [Device]
    public partial class HuePlugDevice : HueOnOffDevice {
        
        public HuePlugDevice(Light light, HueClient hue) : base(hue, light) {
            Type = Helpers.GetTypeString(Helpers.DeviceType.Outlet);
        }

    }

}