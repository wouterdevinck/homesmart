using Newtonsoft.Json;

namespace Home.Devices.HomeWizard.Models {

    public class WaterDataModel {

        [JsonProperty("wifi_ssid")]
        public string WifiSsid { get; set; }

        [JsonProperty("wifi_strength")]
        public int WifiStrength { get; set; }

        [JsonProperty("total_liter_m3")]
        public double TotalLiterM3 { get; set; }

        [JsonProperty("active_liter_lpm")]
        public double ActiveLiterLpm { get; set; }

        [JsonProperty("total_liter_offset_m3")]
        public double TotalLiterOffsetM3 { get; set; }

        public int TotalLiters => (int)(TotalLiterM3 * 1000);

    }

}