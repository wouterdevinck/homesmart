using System.Collections.Generic;
using Newtonsoft.Json;

namespace Home.Devices.Unifi.Models {

    public class PortOverrideRequest {

        public PortOverrideRequest(int port, bool poeEnabled) {
            Overrides = new List<Override> { new() { Port = port, PoeMode = poeEnabled ? "auto" : "off" } };
        }

        [JsonProperty("port_overrides")]
        public List<Override> Overrides { get; set; }

        public class Override {

            [JsonProperty("port_idx")]
            public int Port { get; set; }

            [JsonProperty("poe_mode")]
            public string PoeMode { get; set; }

        }

    }

}