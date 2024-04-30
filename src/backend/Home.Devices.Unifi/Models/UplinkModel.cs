using Newtonsoft.Json;

namespace Home.Devices.Unifi.Models {

    public class UplinkModel {

        [JsonProperty("uplink_mac")]
        public string Mac { get; set; }

        [JsonProperty("uplink_remote_port")]
        public int Port { get; set; }

    }

}
