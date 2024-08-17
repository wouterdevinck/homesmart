using Newtonsoft.Json;

namespace Home.Devices.Zigbee.Models.Requests {

    public class ModeRequest : IZigbeeRequest {

        [JsonProperty("operating_mode")]
        public string Mode { get; set; }

        public ModeRequest(string mode) {
            Mode = mode;
        }

    }

}
