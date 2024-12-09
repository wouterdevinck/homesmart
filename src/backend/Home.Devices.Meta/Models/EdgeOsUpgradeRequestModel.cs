using Newtonsoft.Json;

namespace Home.Devices.Meta.Models {

    public class EdgeOsUpgradeRequestModel {

        [JsonProperty("url")]
        public string Url { get; set; }

    }

}
