using Home.Core;
using Home.Core.Configuration.Models;
using Home.Devices.Unifi.Models;

namespace Home.Devices.Unifi.Devices {

    public class UnifiCamera : UnifiDevice {

        public UnifiCamera(HomeConfigurationModel home, DeviceModel model) : base(home, model, $"UNIFI-PROTECT-{model.Id}") {
            Type = Helpers.GetTypeString(Helpers.DeviceType.Camera);

        }

    }

}
