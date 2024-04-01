using Newtonsoft.Json;

namespace Home.Devices.Unifi.Models {

    public class NetworkDeviceModel : DeviceModel {

        [JsonProperty("ip")]
        public string Ip { get; set; }

        [JsonProperty("uplink")]
        public UplinkModel Uplink { get; set; }

    }

}
