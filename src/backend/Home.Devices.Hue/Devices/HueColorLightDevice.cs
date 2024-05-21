using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Home.Core.Attributes;
using Home.Core.Configuration.Models;
using Home.Core.Devices;
using Home.Core.Models;
using HueApi;
using HueApi.Models;
using HueApi.Models.Requests;

namespace Home.Devices.Hue.Devices {
    
    [Device]
    public partial class HueColorLightDevice : HueLightDevice, IColorLight {
        
        public HueColorLightDevice(Light light, Device device, ZigbeeConnectivity zigbee, LocalHueApi hue, HomeConfigurationModel home) : base(light, device, zigbee, hue, home) {
            if (light.Color != null) {
                ColorXy = new ColorXy {
                    X = light.Color.Xy.X,
                    Y = light.Color.Xy.Y
                };
            }
        }

        [DeviceProperty]
        public ColorXy ColorXy { get; private set; }

        [DeviceCommand]
        public async Task SetColorXyAsync(ColorXy c) {
            var req = new UpdateLight().SetColor(c.X, c.Y);
            var result = await Hue.UpdateLightAsync(HueLightApiId, req);
            if (!result.HasErrors) {
                ColorXy = c;
                NotifyObservers(nameof(ColorXy), ColorXy);
            }
        }

        public new void ProcessUpdate(string type, Dictionary<string, JsonElement> data) {
            if (type == "light" && data.TryGetValue("color", out JsonElement value)) {
                var xy = value.GetProperty("xy");
                var c = new ColorXy {
                    X = xy.GetProperty("x").GetDouble(),
                    Y = xy.GetProperty("y").GetDouble()
                };
                if (ColorXy != c) {
                    ColorXy = c;
                    NotifyObservers(nameof(ColorXy), ColorXy);
                }
            }
            base.ProcessUpdate(type, data);
        }

    }

}