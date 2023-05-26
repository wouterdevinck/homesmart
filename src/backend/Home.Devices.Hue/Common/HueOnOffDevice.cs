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

namespace Home.Devices.Hue.Common {

    [Device]
    public abstract partial class HueOnOffDevice : HueDevice, IOnOffDevice {

        protected HueOnOffDevice(Light light, Device device, ZigbeeConnectivity zigbee, LocalHueApi hue, HomeConfigurationModel home) : base(hue, device.Id, home, $"HUE-LIGHT-{zigbee.MacAddress}") {
            Name = device.Metadata.Name;
            Reachable = zigbee.Status == ConnectivityStatus.connected;
            On = light.On.IsOn && Reachable;
            Manufacturer = device.ProductData.ManufacturerName.HarmonizeManufacturer();
            Model = device.ProductData.ModelId.HarmonizeModel();
            Version = device.ProductData.SoftwareVersion;
            if (string.IsNullOrEmpty(Version) || Version == "0.0.0") Version = Helpers.VersionNotAvailable;
            HueApiId = light.Id;
        }

        [DeviceProperty]
        public bool On { get; protected set; }

        [DeviceCommand]
        public async Task TurnOnAsync() {
            var req = new UpdateLight().TurnOn();
            var result = await Hue.UpdateLightAsync(HueApiId, req);
            if (!result.HasErrors) {
                On = true;
                NotifyObservers(nameof(On), On);
            }
        }

        [DeviceCommand]
        public async Task TurnOffAsync() {
            var req = new UpdateLight().TurnOff();
            var result = await Hue.UpdateLightAsync(HueApiId, req);
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

        public void ProcessUpdate(Dictionary<string, JsonElement> data) {
            if (data.TryGetValue("on", out JsonElement value)) {
                var on = value.GetProperty("on").GetBoolean();
                if (On != on) {
                    On = on;
                    NotifyObservers(nameof(On), On);
                }
            }
            if (data.TryGetValue("status", out JsonElement statusValue)) {
                var r = statusValue.GetString() == "connected";
                if (Reachable != r) {
                    Reachable = r;
                    NotifyObservers(nameof(Reachable), Reachable);
                    if (!Reachable && On) {
                        On = false;
                        NotifyObservers(nameof(On), On);
                    }
                    if (Reachable) {
                        Hue.GetLightAsync(HueApiId).ContinueWith(x => {
                            if (!x.Result.HasErrors) {
                                var on = x.Result.Data.SingleOrDefault().On.IsOn;
                                if (On != on) {
                                    On = on;
                                    NotifyObservers(nameof(On), On);
                                }
                            }
                        });
                        // TODO Can brightness also update in the scenario? E.g. power on behavior with full brightness?
                    }
                }
            }
        }

    }

}