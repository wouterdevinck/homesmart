using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Home.Core;
using Home.Core.Attributes;
using Home.Core.Configuration.Models;
using Home.Core.Devices;
using HueApi;
using HueApi.Models;
using HueApi.Models.Requests;

namespace Home.Devices.Hue.Devices {

    [Device]
    public partial class HueLightDevice : HueDevice, IDimmableLight {

        protected Guid HueLightApiId;

        [DeviceProperty]
        public bool On { get; protected set; }
        
        [DeviceProperty]
        public double Brightness { get; protected set; }

        public HueLightDevice(Light light, Device device, ZigbeeConnectivity zigbee, LocalHueApi hue, HomeConfigurationModel home) : base(hue, device.Id, home, device, zigbee, $"HUE-LIGHT-{zigbee.MacAddress}") {
            if (light.Dimming != null) Brightness = light.Dimming.Brightness;
            Type = Helpers.GetTypeString(Helpers.DeviceType.Light);
            On = light.On.IsOn && Reachable;
            HueLightApiId = light.Id;
        }

        [DeviceCommand]
        public async Task TurnOnAsync() {
            var req = new UpdateLight().TurnOn();
            var result = await Hue.UpdateLightAsync(HueLightApiId, req);
            if (!result.HasErrors) {
                On = true;
                NotifyObservers(nameof(On), On);
            }
        }

        [DeviceCommand]
        public async Task TurnOffAsync() {
            var req = new UpdateLight().TurnOff();
            var result = await Hue.UpdateLightAsync(HueLightApiId, req);
            if (!result.HasErrors) {
                On = false;
                NotifyObservers(nameof(On), On);
            }
        }

        [DeviceCommand]
        public Task ToggleOnOffAsync() {
            if (On) {
                return TurnOffAsync();
            }
            return TurnOnAsync();
        }

        [DeviceCommand]
        public async Task SetBrightnessAsync(double bri) {
            var req = new UpdateLight().SetBrightness(bri);
            var result = await Hue.UpdateLightAsync(HueLightApiId, req);
            if (!result.HasErrors) {
                Brightness = bri;
                NotifyObservers(nameof(Brightness), Brightness);
            }
        }

        public new void ProcessUpdate(string type, Dictionary<string, JsonElement> data) {
            var reachableBefore = Reachable;
            if (type == "light" && data.TryGetValue("on", out JsonElement onValue)) {
                var on = onValue.GetProperty("on").GetBoolean();
                if (On != on) {
                    On = on;
                    NotifyObservers(nameof(On), On);
                }
            }
            if (type == "light" && data.TryGetValue("dimming", out JsonElement briValue)) {
                var bri = briValue.GetProperty("brightness").GetDouble();
                if (Math.Abs(Brightness - bri) >= Tolerance) {
                    Brightness = bri;
                    NotifyObservers(nameof(Brightness), Brightness);
                }
            }
            base.ProcessUpdate(type, data);
            if (reachableBefore && !Reachable && On) {
                On = false;
                NotifyObservers(nameof(On), On);
            }
            if (!reachableBefore && Reachable) {
                Hue.GetLightAsync(HueLightApiId).ContinueWith(x => {
                    if (!x.Result.HasErrors) {
                        var light = x.Result.Data.SingleOrDefault();
                        if (light != null) {
                            var on = light.On.IsOn;
                            if (On != on) {
                                On = on;
                                NotifyObservers(nameof(On), On);
                            }
                            if (light.Dimming != null) {
                                var bri = light.Dimming.Brightness; 
                                if (Math.Abs(Brightness - bri) >= Tolerance) {
                                    Brightness = bri;
                                    NotifyObservers(nameof(Brightness), Brightness);
                                }
                            }
                        }
                    }
                });
            }
        }
        
    }

}