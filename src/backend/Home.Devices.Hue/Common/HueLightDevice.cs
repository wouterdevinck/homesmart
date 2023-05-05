using System.Collections.Generic;
using System.Threading.Tasks;
using Home.Core;
using Home.Core.Attributes;
using Home.Core.Configuration.Models;
using Home.Core.Devices;
using Q42.HueApi;

namespace Home.Devices.Hue.Common {

    [Device]
    public abstract partial class HueLightDevice : HueOnOffDevice, IDimmableLight {

        public HueLightDevice(Light light, HueClient hue, HomeConfigurationModel home) : base(hue, light, home) {
            Brightness = light.State.Brightness;
            Type = Helpers.GetTypeString(Helpers.DeviceType.Light);
        }

        [DeviceProperty]
        public byte Brightness { get; protected set; }

        [DeviceCommand]
        public async Task SetBrightnessAsync(byte bri) {
            var command = new LightCommand { Brightness = bri };
            var result = await Hue.SendCommandAsync(command, new List<string> { LocalId });
            if (result[0].Error == null) {
                Brightness = bri;
                NotifyObservers("brightness", Brightness);
            }
        }

    }

}