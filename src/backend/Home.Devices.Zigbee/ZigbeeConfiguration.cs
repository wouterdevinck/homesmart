using Home.Core.Configuration.Interfaces;

namespace Home.Devices.Zigbee {

    public class ZigbeeConfiguration : IProviderConfiguration {

        public string Ip { get; set; }
        public string BaseTopic { get; set; }

    }

}
