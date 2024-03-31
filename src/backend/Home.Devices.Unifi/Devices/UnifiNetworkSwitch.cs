using Home.Core;
using Home.Core.Configuration.Models;
using Home.Devices.Unifi.Models;

namespace Home.Devices.Unifi.Devices {

    public class UnifiNetworkSwitch : UnifiDevice {

        public UnifiNetworkSwitch(HomeConfigurationModel home, DeviceModel model) : base(home, model, $"UNIFI-NETWORK-{model.Id}") {
            Type = Helpers.GetTypeString(Helpers.DeviceType.NetworkSwitch);

        }

    }

}
