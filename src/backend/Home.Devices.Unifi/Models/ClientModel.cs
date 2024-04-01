using Newtonsoft.Json;

namespace Home.Devices.Unifi.Models {

    public class ClientModel {

        [JsonProperty("mac")]
        public string Mac { get; set; }

        [JsonProperty("ip")]
        public string Ip { get; set; }

        [JsonProperty("sw_port")]
        public string UplinkPort { get; set; }

    }

}
