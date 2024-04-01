using Home.Core;
using Home.Core.Attributes;
using Home.Core.Configuration.Models;
using Home.Devices.Unifi.Models;

namespace Home.Devices.Unifi {

    public class UnifiDevice : AbstractDevice {

        [DeviceProperty]
        public string Ip { get; protected set; }

        [DeviceProperty]
        public string Mac { get; private set; }

        [DeviceProperty]
        public string UplinkMac { get; protected set; }

        [DeviceProperty]
        public string UplinkPort { get; protected set; }

        public UnifiDevice(HomeConfigurationModel home, DeviceModel device, string id) : base(home, id) {
            Manufacturer = Helpers.Ubiquiti.HarmonizeManufacturer();
            Version = device.Version;
            Name = device.Name;
            Model = device.Name;
            Reachable = device.Online;
            Mac = device.Mac;
        }

    }

}
