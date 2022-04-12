using System.Collections.Generic;
using System.Threading.Tasks;
using Home.Core;
using Home.Core.Devices;
using Q42.HueApi;

namespace Home.Devices.Hue.Common {

    public abstract class HueLightDevice : HueOnOffDevice, IDimmableLight {

        public HueLightDevice(Light light, HueClient hue) : base(hue, light) {
            Brightness = light.State.Brightness;
            Type = Helpers.GetTypeString(Helpers.DeviceType.Light);
        }
        
        public byte Brightness { get; protected set; }

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