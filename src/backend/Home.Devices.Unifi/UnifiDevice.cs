using Home.Core;
using Home.Core.Attributes;
using Home.Core.Configuration.Models;
using Home.Devices.Unifi.Models;
using Newtonsoft.Json;

namespace Home.Devices.Unifi {

    [Device]
    public partial class UnifiDevice : AbstractDevice {

        [DeviceProperty]
        public string Ip { get; protected set; }

        [DeviceProperty]
        public string Mac { get; private set; }

        [JsonIgnore]
        public string UplinkMac { get; protected set; }

        [JsonIgnore]
        public int UplinkPort { get; protected set; }

        public UnifiDevice(HomeConfigurationModel home, DeviceModel device, string id) : base(home, id) {
            Manufacturer = Helpers.Ubiquiti.HarmonizeManufacturer();
            Version = device.Version;
            Name = device.Name;
            Model = device.Model;
            Reachable = device.Online;
            Mac = device.Mac;
        }

    }

}
