using Newtonsoft.Json;

namespace Home.Devices.Zigbee.Models {

    public class DeviceModel {

        [JsonProperty("ieee_address")]
        public string Id { get; set; }

        [JsonProperty("friendly_name")]
        public string Name { get; set; }

        [JsonProperty("definition")]
        public DeviceDefinitionModel Definition { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("date_code")]
        public string VersionDate { get; set; }
        
        [JsonProperty("software_build_id")]
        public string VersionBuild { get; set; }

        public string Version {
            get {
                if (Definition.Manufacturer == "Xiaomi") {
                    return VersionDate;
                }
                return VersionBuild;
            }
        }

        [JsonProperty("model_id")]
        public string ModelId { get; set; }

        public string Model {
            get {
                if (Definition.Manufacturer == "Xiaomi" || Definition.Manufacturer == "IKEA") {
                    return Definition.Model;
                }
                return ModelId;
            }
        }

        [JsonProperty("interview_completed")]
        public bool InterviewCompleted { get; set; }

        [JsonProperty("power_source")]
        public string PowerSource { get; set; }

    }

}