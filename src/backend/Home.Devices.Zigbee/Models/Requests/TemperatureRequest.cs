using Newtonsoft.Json;

namespace Home.Devices.Zigbee.Models.Requests {

    public class TemperatureRequest : IZigbeeRequest {

        [JsonProperty("occupied_heating_setpoint")]
        public double Temperature { get; set; }

        public TemperatureRequest(double temperature) {
            Temperature = temperature;
        }

    }

}
