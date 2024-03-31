using Home.Core;
using Home.Core.Configuration.Models;
using Home.Devices.Unifi.Models;

namespace Home.Devices.Unifi {

    public class UnifiDevice : AbstractDevice {

        public UnifiDevice(HomeConfigurationModel home, DeviceModel model, string id) : base(home, id) {
            Manufacturer = Helpers.Ubiquiti.HarmonizeManufacturer();
            Version = model.Version;
            Name = model.Name;
            Model = model.Name;
            Reachable = model.Online;
        }

    }

}
