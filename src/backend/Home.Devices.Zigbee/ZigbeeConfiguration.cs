using Home.Core.Configuration.Interfaces;

namespace Home.Devices.Zigbee {

    public class ZigbeeConfiguration : IDeviceProviderConfiguration {

        public string Ip { get; set; }
        public string BaseTopic { get; set; }

    }

}
