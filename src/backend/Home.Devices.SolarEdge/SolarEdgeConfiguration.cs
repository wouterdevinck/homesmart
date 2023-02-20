using Home.Core.Configuration.Interfaces;

namespace Home.Devices.SolarEdge {

    public class SolarEdgeConfiguration : IDeviceProviderConfiguration {

        public string Site { get; set; }
        public string ApiKey { get; set; }

    }

}
