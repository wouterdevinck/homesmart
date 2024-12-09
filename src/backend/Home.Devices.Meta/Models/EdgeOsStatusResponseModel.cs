using Newtonsoft.Json;

namespace Home.Devices.Meta.Models {

    public class EdgeOsStatusResponseModel {

        [JsonProperty("edgeos_version")]
        public string EdgeOsVersion { get; set; }

        [JsonProperty("booted_from")]
        public string BootedFrom { get; set; }

        [JsonProperty("application_name")]
        public string AppName { get; set; }

        [JsonProperty("application_version")]
        public string AppVersion { get; set; }

    }

}
