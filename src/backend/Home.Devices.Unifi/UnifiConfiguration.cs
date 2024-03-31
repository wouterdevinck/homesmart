using Home.Core.Configuration.Interfaces;

namespace Home.Devices.Unifi {

    public class UnifiConfiguration : IProviderConfiguration {

        public string Ip { get; set; }
        public string Site { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

    }

}
