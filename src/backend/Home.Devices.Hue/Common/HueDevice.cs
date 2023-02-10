using Home.Core;
using Home.Core.Configuration.Models;
using Q42.HueApi;

namespace Home.Devices.Hue.Common {

    public abstract class HueDevice : AbstractDevice {
        
        protected readonly HueClient Hue;
        protected readonly string LocalId;

        protected HueDevice(HueClient hue, string localId, HomeConfigurationModel home, string id) : base(home, id) {
            Hue = hue;
            LocalId = localId;
        }

    }

}