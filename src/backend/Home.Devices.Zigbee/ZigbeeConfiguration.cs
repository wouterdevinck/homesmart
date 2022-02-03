using Home.Core.Configuration;

namespace Home.Devices.Zigbee {

    public class ZigbeeConfiguration : IDeviceProviderConfiguration {

        public string Ip { get; set; }
        public string BaseTopic { get; set; }

    }

}
