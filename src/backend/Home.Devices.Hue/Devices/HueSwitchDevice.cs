using System;
using System.Collections.Generic;
using System.Text.Json;
using Home.Core;
using Home.Core.Attributes;
using Home.Core.Configuration.Models;
using Home.Core.Devices;
using HueApi;
using HueApi.Models;

namespace Home.Devices.Hue.Devices {

    [Device]
    public partial class HueSwitchDevice : HueDevice, IBatteryDevice {

        [DeviceProperty<double>(Unit = "%", Min = 0, Max = 100)]
        public double Battery { get; private set; }

        //    [DeviceProperty]
        //    public int ButtonEvent { get; private set; }

        //    [DeviceProperty]
        //    public DateTime SensorUpdate { get; private set; }

        public HueSwitchDevice(List<ButtonResource> _, List<RelativeRotaryResource> __, Device device, ZigbeeConnectivity zigbee, DevicePower pwr, LocalHueApi hue, HomeConfigurationModel home) : base(hue, device.Id, home, device, zigbee, $"HUE-SENSOR-{zigbee.MacAddress}") {
            Type = Helpers.GetTypeString(Helpers.DeviceType.Switch);
            if (pwr.PowerState is { BatteryLevel: not null }) Battery = (double)pwr.PowerState.BatteryLevel;
            // TODO Implement buttons
        }

        public new void ProcessUpdate(string type, Dictionary<string, JsonElement> data) {
            // if (type == "relative_rotary" && data.TryGetValue("relative_rotary", out JsonElement rotaryValue)) {}
            // if (type == "button" && data.TryGetValue("button", out JsonElement buttonValue)) {}
            if (type == "device_power" && data.TryGetValue("power_state", out JsonElement powerValue)) {
                var pwr = powerValue.Deserialize<PowerState>();
                if(pwr.BatteryLevel != null && Math.Abs(Battery - (double)pwr.BatteryLevel) >= Tolerance) {
                    if (pwr.BatteryLevel != null) Battery = (double)(pwr.BatteryLevel);
                    NotifyObservers(nameof(Battery), Battery);
                }
            }
            base.ProcessUpdate(type, data);
        }

    }

}