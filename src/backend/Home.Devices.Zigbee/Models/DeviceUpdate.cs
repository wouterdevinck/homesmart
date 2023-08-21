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

        [JsonProperty("local_temperature")]
        public double LocalTemperature { get; set; }

        [JsonProperty("occupied_heating_setpoint")]
        public double RequestedTemperature { get; set; }

        [JsonProperty("water_leak")]
        public bool WaterLeak { get; set; }

        [JsonProperty("child_lock")]
        public string ChildLock { get; set; }

        [JsonProperty("current")]
        public double Current { get; set; }

        [JsonProperty("power")]
        public double Power { get; set; }

        [JsonProperty("energy")]
        public double Energy { get; set; }

        [JsonProperty("voltage")]
        public double Voltage { get; set; }

        [JsonProperty("pi_heating_demand")]
        public int ValvePosition { get; set; }

        [JsonProperty("running_state")]
        public string RunningState { get; set; }

        [JsonProperty("system_mode")]
        public string SystemMode { get; set; }
        
        // Not implemented
        //   "device_temperature"
        //   "indicator_mode":"off"
        //   "power_outage_memory":"on"

        // Not implemented for Bosch TRV
        //   * "boost": "OFF"
        //   * "display_brightness": 10
        //   * "display_ontime": 10
        //   * "display_orientation": "flipped"
        //   * "displayed_temperature": "measured"
        //   * "local_temperature_calibration": 0
        //   * "remote_temperature": 0
        //   * "window_open": "OFF"

        // Not implemented for any device
        //   * "update": { ... }
        //   * "linkquality": 48

    }

}