using Home.Core;
using Home.Devices.Hue.Common;
using Q42.HueApi;

namespace Home.Devices.Hue.Devices {

    public partial class HuePlugDevice : HueOnOffDevice {
        
        public HuePlugDevice(Light light, HueClient hue) : base(hue, light) {
            Type = Helpers.GetTypeString(Helpers.DeviceType.Outlet);
        }

    }

}