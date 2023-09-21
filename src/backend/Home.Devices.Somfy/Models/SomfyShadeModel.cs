using System.Collections.Generic;
using Newtonsoft.Json;

namespace Home.Devices.Somfy.Models {

    public class SomfyShadeModel {

        [JsonProperty("shadeId")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("position")]
        public int Position { get; set; }

        [JsonProperty("linkedRemotes")]
        public List<SomfyRemoteModel> Remotes { get; set; }

    }

    public class SomfyRemoteModel {

        [JsonProperty("remoteAddress")]
        public string Address { get; set; }

    }

}
