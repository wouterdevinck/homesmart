using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Home.Core.Attributes;
using Home.Core.Configuration.Models;
using Home.Core.Devices;
using HueApi;
using HueApi.Models;
using HueApi.Models.Requests;

namespace Home.Devices.Hue.Devices {

    [Device]
    public partial class HueTemperatureLightDevice : HueLightDevice, IColorTemperatureLight {

        public HueTemperatureLightDevice(Light light, Device device, ZigbeeConnectivity zigbee, LocalHueApi hue,
            HomeConfigurationModel home) : base(light, device, zigbee, hue, home) {
            if (light.ColorTemperature != null) ColorTemperature = light.ColorTemperature.Mirek ?? 0;
        }

        [DeviceProperty<int>(Unit = "mirek", Min = 153, Max = 500)]
        public int ColorTemperature { get; private set; }

        [DeviceCommand]
        public async Task SetColorTemperatureAsync(int ct) {
            var req = new UpdateLight().SetColor(ct);
            var result = await Hue.UpdateLightAsync(HueLightApiId, req);
            if (!result.HasErrors) {
                ColorTemperature = ct;
                NotifyObservers(nameof(ColorTemperature), ColorTemperature);
            }
        }

        public new void ProcessUpdate(string type, Dictionary<string, JsonElement> data) {
            var reachableBefore = Reachable;
            if (type == "light" && data.TryGetValue("color_temperature", out JsonElement value)) {
                if (value.GetProperty("mirek_valid").GetBoolean()) {
                    var ct = value.GetProperty("mirek").GetInt32();
                    if (ColorTemperature != ct) {
                        ColorTemperature = ct;
                        NotifyObservers(nameof(ColorTemperature), ColorTemperature);
                    }
                } else {
                    if (ColorTemperature != 0) {
                        ColorTemperature = 0;
                        NotifyObservers(nameof(ColorTemperature), ColorTemperature);
                    }
                }
            }
            base.ProcessUpdate(type, data);
            if (!reachableBefore && Reachable) {
                Hue.GetLightAsync(HueLightApiId).ContinueWith(x => {
                    if (!x.Result.HasErrors) {
                        var light = x.Result.Data.SingleOrDefault();
                        if (light != null) {
                            if (light.ColorTemperature != null) {
                                var ct = light.ColorTemperature.Mirek ?? 0;
                                if (ColorTemperature != ct) {
                                    ColorTemperature = ct;
                                    NotifyObservers(nameof(ColorTemperature), ColorTemperature);
                                }
                            }
                        }
                    }
                });
            }

        }

    }

}