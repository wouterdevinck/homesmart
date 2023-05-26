using System.Collections.Generic;
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
    public abstract partial class HueLightDevice : HueOnOffDevice, IDimmableLight {

        public HueLightDevice(Light light, Device device, ZigbeeConnectivity zigbee, LocalHueApi hue, HomeConfigurationModel home) : base(light, device, zigbee, hue, home) {
            Brightness = light.Dimming.Brightness.MapToByte();
            Type = Helpers.GetTypeString(Helpers.DeviceType.Light);
        }

        [DeviceProperty]
        public byte Brightness { get; protected set; }

        [DeviceCommand]
        public async Task SetBrightnessAsync(byte bri) {
            var req = new UpdateLight().SetBrightness(bri.MapFromByte());
            var result = await Hue.UpdateLightAsync(HueApiId, req);
            if (!result.HasErrors) {
                Brightness = bri;
                NotifyObservers(nameof(Brightness), Brightness);
            }
        }

        public new void ProcessUpdate(Dictionary<string, JsonElement> data) {
            if (data.TryGetValue("dimming", out JsonElement value)) {
                var bri = value.GetProperty("brightness").GetDouble().MapToByte();
                if (Brightness != bri) {
                    Brightness = bri;
                    NotifyObservers(nameof(Brightness), Brightness);
                }
            }
            base.ProcessUpdate(data);
        }

    }

}