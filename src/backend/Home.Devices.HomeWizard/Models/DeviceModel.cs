using Newtonsoft.Json;

namespace Home.Devices.HomeWizard.Models {

    public class DeviceModel {

        [JsonProperty("product_name")]
        public string ProductName { get; set; }

        [JsonProperty("product_type")]
        public string ProductType { get; set; }

        [JsonProperty("serial")]
        public string Serial { get; set; }

        [JsonProperty("firmware_version")]
        public string FirmwareVersion { get; set; }

        [JsonProperty("api_version")]
        public string ApiVersion { get; set; }

    }

}