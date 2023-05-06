using System.Collections.Generic;
using Home.Core.Configuration.Interfaces;

namespace Home.Devices.Tuya {

    public class TuyaConfiguration : IDeviceProviderConfiguration {

        public List<TuyaDeviceConfiguration> Devices { get; set; }

    }

    public class TuyaDeviceConfiguration {

        public string Name { get; set; }
        public string Type { get; set; }
        public string Ip { get; set; }
        public string Id { get; set; }
        public string Key { get; set; }

    }

}
