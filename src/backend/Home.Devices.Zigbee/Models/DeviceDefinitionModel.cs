using Newtonsoft.Json;

namespace Home.Devices.Zigbee.Models {

    public class DeviceDefinitionModel {

        [JsonProperty("vendor")]
        public string Manufacturer { get; set; }

        [JsonProperty("model")]
        public string Model { get; set; }

    }

}