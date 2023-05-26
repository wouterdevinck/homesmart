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

        public HueSwitchDevice(List<ButtonResource> _, Device device, ZigbeeConnectivity zigbee, DevicePower pwr, LocalHueApi hue, HomeConfigurationModel home) : base(hue, device.Id, home, $"HUE-SENSOR-{zigbee.MacAddress}") {
            Name = device.Metadata.Name;
            Manufacturer = device.ProductData.ManufacturerName.HarmonizeManufacturer();
            Model = device.ProductData.ModelId.HarmonizeModel();
            Version = device.ProductData.SoftwareVersion;
            Type = Helpers.GetTypeString(Helpers.DeviceType.Switch);
            Reachable = zigbee.Status == ConnectivityStatus.connected;
            Battery = pwr.ExtensionData["power_state"].GetProperty("battery_level").GetDouble();
            // TODO Implement buttons
        }

        public void ProcessUpdate(Dictionary<string, JsonElement> data) {
            // TODO Test battery update - can take a while?
            //if (data.TryGetValue("on", out JsonElement value)) {
            //    var on = value.GetProperty("on").GetBoolean();
            //    if (Battery != on) {
            //        Battery = on;
            //        NotifyObservers(nameof(Battery), Battery);
            //    }
            //}
            if (data.TryGetValue("status", out JsonElement statusValue)) {
                var r = statusValue.GetString() == "connected";
                if (Reachable != r) {
                    Reachable = r;
                    NotifyObservers(nameof(Reachable), Reachable);
                }
            }
        }

    }

}