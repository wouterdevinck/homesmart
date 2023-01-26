using System;
using Newtonsoft.Json;

namespace Home.Devices.Zigbee.Models {

    public class DeviceUpdate {

        [JsonProperty("last_seen")]
        public DateTime LastSeen { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("battery")]
        public double Battery { get; set; }

        [JsonProperty("brightness")]
        public byte Brightness { get; set; }

        [JsonProperty("action")]
        public string Action { get; set; }

        [JsonProperty("humidity")]
        public double Humidity { get; set; }

        [JsonProperty("pressure")]
        public double Pressure { get; set; }

        [JsonProperty("temperature")]
        public double Temperature { get; set; }

        [JsonProperty("water_leak")]
        public bool WaterLeak { get; set; }

    }

}