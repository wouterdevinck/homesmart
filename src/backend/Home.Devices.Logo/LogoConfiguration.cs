using System.Collections.Generic;
using Home.Core.Configuration.Interfaces;

namespace Home.Devices.Logo {

    public class LogoConfiguration : IDeviceProviderConfiguration {

        public string Ip { get; set; }
        public int Port { get; set; }
        public int PollingInterval { get; set; }
        public int OutputsAddress { get; set; }
        public int SwitchReturnTime { get; set; }
        public List<LogoDeviceConfiguration> Devices { get; set; }

    }

    public class LogoDeviceConfiguration {

        public string Name { get; set; }
        public int SwitchAddress { get; set; }
        public int OutputNumber { get; set; }

    }

}
