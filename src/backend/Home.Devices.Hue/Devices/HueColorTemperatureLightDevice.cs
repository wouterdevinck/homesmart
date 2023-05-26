using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Home.Core.Attributes;
using Home.Core.Configuration.Models;
using Home.Core.Devices;
using Home.Devices.Hue.Common;
using HueApi;
using HueApi.Models;
using HueApi.Models.Requests;

namespace Home.Devices.Hue.Devices {

    [Device]
    public partial class HueColorTemperatureLightDevice : HueLightDevice, IColorTemperatureLight {
        
        public HueColorTemperatureLightDevice(Light light, Device device, ZigbeeConnectivity zigbee, LocalHueApi hue, HomeConfigurationModel home) : base(light, device, zigbee, hue, home) {
            ColorTemperature = light.ColorTemperature.Mirek ?? 0;
        }

        [DeviceProperty]
        public int ColorTemperature { get; private set; }

        [DeviceCommand]
        public async Task SetColorTemperature(int ct) {
            var req = new UpdateLight().SetColor(ct);
            var result = await Hue.UpdateLightAsync(HueApiId, req);
            if (!result.HasErrors) {
                ColorTemperature = ct;
                NotifyObservers(nameof(ColorTemperature), ColorTemperature);
            }
        }

        public new void ProcessUpdate(Dictionary<string, JsonElement> data) {
            if (data.TryGetValue("color_temperature", out JsonElement value)) {
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
            base.ProcessUpdate(data);
        }

    }

}