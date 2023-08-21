using Newtonsoft.Json;

namespace Home.Devices.Zigbee.Models.Requests {

    public class StateRequest : IZigbeeRequest {

        [JsonProperty("state")]
        public string State { get; set; }

        public StateRequest(string state) {
            State = state;
        }

    }

}
