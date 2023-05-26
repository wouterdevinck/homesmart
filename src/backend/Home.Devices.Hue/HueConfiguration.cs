using Home.Core.Configuration.Interfaces;

namespace Home.Devices.Hue {

    public class HueConfiguration : IProviderConfiguration {

        public string Ip { get; set; }
        public string ApiKey { get; set; }

    }

}
