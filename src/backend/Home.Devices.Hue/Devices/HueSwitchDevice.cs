using System;
using System.Collections.Generic;
using System.Text.Json;
using Home.Core;
using Home.Core.Attributes;
using Home.Core.Configuration.Models;
using Home.Core.Devices;
using Home.Devices.Hue.Common;
using HueApi;
using HueApi.Models;

namespace Home.Devices.Hue.Devices {

    [Device]
    public partial class HueSwitchDevice : HueDevice, IBatteryDevice {
    
        [DeviceProperty]
        public double Battery { get; private set; }

        //    [DeviceProperty]
        //    public int ButtonEvent { get; private set; }

        //    [DeviceProperty]
        //    public DateTime SensorUpdate { get; private set; }

        public HueSwitchDevice(List<ButtonResource> _, List<RelativeRotaryResource> __, Device device, ZigbeeConnectivity zigbee, DevicePower pwr, LocalHueApi hue, HomeConfigurationModel home) : base(hue, device.Id, home, device, zigbee, $"HUE-SENSOR-{zigbee.MacAddress}") {
            Type = Helpers.GetTypeString(Helpers.DeviceType.Switch);
            Battery = (double)pwr.PowerState.BatteryLevel;
            // TODO Implement buttons
        }

        public void ProcessUpdate(Guid id, Dictionary<string, JsonElement> data) {
            if (data.TryGetValue("relative_rotary", out JsonElement rotaryValue)) {
                // TODO
            }
            if (data.TryGetValue("button", out JsonElement buttonValue)) {
                // TODO
            }
            if (data.TryGetValue("status", out JsonElement statusValue)) { // TODO Move to device?
                var r = statusValue.GetString() == "connected";
                if (Reachable != r) {
                    Reachable = r;
                    NotifyObservers(nameof(Reachable), Reachable);
                }
            }
            if (data.TryGetValue("power_state", out JsonElement powerValue)) {
                var pwr = powerValue.Deserialize<PowerState>();
                if(pwr != null && pwr.BatteryLevel != Battery) {
                    Battery = (double)(pwr.BatteryLevel);
                    NotifyObservers(nameof(Battery), Battery);
                }
            }
        }

    }

}