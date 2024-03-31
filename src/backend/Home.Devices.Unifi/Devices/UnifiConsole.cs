using Home.Core;
using Home.Core.Configuration.Models;
using Home.Devices.Unifi.Models;

namespace Home.Devices.Unifi.Devices {

    public class UnifiConsole : UnifiDevice {

        public UnifiConsole(HomeConfigurationModel home, DeviceModel model) : base(home, model, $"UNIFI-NETWORK-{model.Id}") {
            Type = Helpers.GetTypeString(Helpers.DeviceType.Hub);

        }

    }

}
