using Home.Core.Configuration;

namespace Home.Devices.Hue {

    public class HueConfiguration : IDeviceProviderConfiguration {

        public string ApiKey { get; set; }
        public PollingConfiguration Polling { get; set; }

    }

    public class PollingConfiguration {

        public uint MinPollInterval { get; set; }
        public uint MaxPollInterval { get; set; }
        public uint BackOffInterval { get; set; }
        public uint BackOffFactor { get; set; }

    }

}
