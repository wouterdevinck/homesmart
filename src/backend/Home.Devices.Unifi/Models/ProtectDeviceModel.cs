using Newtonsoft.Json;

namespace Home.Devices.Unifi.Models {

    public class ProtectDeviceModel : DeviceModel {

        [JsonProperty("uplink_mac")]
        public string UplinkMac { get; set; }

    }

}
