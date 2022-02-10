using Home.Core.Configuration;

namespace Home.Devices.Hue {

    public class HueConfiguration : IDeviceProviderConfiguration {

        public string ApiKey { get; set; }

    }

}
