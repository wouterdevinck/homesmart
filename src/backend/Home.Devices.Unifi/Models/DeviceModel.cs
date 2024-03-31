using Newtonsoft.Json;

namespace Home.Devices.Unifi.Models {

    public class DeviceModel {

        [JsonProperty("_id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("model")]
        public string Model { get; set; }

        [JsonProperty("displayable_version")]
        public string Version { get; set; }

        [JsonProperty("state")]
        public int State { get; set; }

        public bool Online => State == 1;

    }

}
