using System.Collections.Generic;
using Newtonsoft.Json;

namespace Home.Devices.Unifi.Models {

    public class DevicesResponse {

        [JsonProperty("network_devices")]
        public List<NetworkDeviceModel> NetworkDevices { get; set; }

        [JsonProperty("protect_devices")]
        public List<ProtectDeviceModel> ProtectDevices { get; set; }

    }

}
