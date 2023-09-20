using Newtonsoft.Json;

namespace Home.Devices.Somfy.Models {

    public class SomfyCommandModel {

        [JsonProperty("sourceAddress")]
        public string Address { get; set; }

        [JsonProperty("cmd")]
        public string Command { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }

        public bool IsFromRemote() {
            return Source == "remote";
        }

    }

}
