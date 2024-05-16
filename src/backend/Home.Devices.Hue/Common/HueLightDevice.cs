using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Home.Core;
using Home.Core.Attributes;
using Home.Core.Configuration.Models;
using Home.Core.Devices;
using Home.Devices.Hue.Utilities;
using HueApi;
using HueApi.Models;
using HueApi.Models.Requests;

namespace Home.Devices.Hue.Common {

    [Device]
    public partial class HueLightDevice : HueDevice, IDimmableLight {

        protected Guid HueLightApiId;

        [DeviceProperty]
        public bool On { get; protected set; }
        
        [DeviceProperty]
        public byte Brightness { get; protected set; }

        public HueLightDevice(Light light, Device device, ZigbeeConnectivity zigbee, LocalHueApi hue, HomeConfigurationModel home) : base(hue, device.Id, home, device, zigbee, $"HUE-LIGHT-{zigbee.MacAddress}") {
            if (light.Dimming != null) Brightness = light.Dimming.Brightness.MapToByte();
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
        public async Task SetBrightnessAsync(byte bri) {
            var req = new UpdateLight().SetBrightness(bri.MapFromByte());
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
                var bri = briValue.GetProperty("brightness").GetDouble().MapToByte();
                if (Brightness != bri) {
                    Brightness = bri;
                    NotifyObservers(nameof(Brightness), Brightness);
                }
            }
            if (type == "zigbee_connectivity" && data.TryGetValue("status", out JsonElement statusValue)) {
                var r = statusValue.GetString() == "connected";
                if (Reachable != r) {
                    Reachable = r;
                    NotifyObservers(nameof(Reachable), Reachable);
                    if (!Reachable && On) {
                        On = false;
                        NotifyObservers(nameof(On), On);
                    }
                }
            }
            base.ProcessUpdate(type, data);
            if (!reachableBefore && Reachable) {
                Hue.GetLightAsync(HueLightApiId).ContinueWith(x => {
                    if (!x.Result.HasErrors) {
                        var on = x.Result.Data.SingleOrDefault()?.On.IsOn;
                        if (on != null && On != on) {
                            On = on.Value;
                            NotifyObservers(nameof(On), On);
                        }
                    }
                });
                // TODO Can brightness also update in the scenario? E.g. power on behavior with full brightness?
            }
        }
        
    }

}