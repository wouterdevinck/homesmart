using Home.Core;
using Q42.HueApi;

namespace Home.Devices.Hue.Common {

    public abstract class HueDevice : AbstractDevice {
        
        protected readonly HueClient Hue;
        protected readonly string LocalId;

        protected HueDevice(HueClient hue, string localId) {
            Hue = hue;
            LocalId = localId;
        }

    }

}