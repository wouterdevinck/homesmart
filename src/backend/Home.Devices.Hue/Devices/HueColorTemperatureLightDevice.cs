using System.Collections.Generic;
using System.Threading.Tasks;
using Home.Devices.Hue.Common;
using Q42.HueApi;

namespace Home.Devices.Hue.Devices {

    public partial class HueColorTemperatureLightDevice : HueLightDevice {
        
        public HueColorTemperatureLightDevice(Light light, HueClient hue) : base(light, hue) {
            if (light.State.ColorTemperature != null) ColorTemperature = light.State.ColorTemperature.Value;
        }

        public int ColorTemperature { get; private set; }

        public async Task SetColorTemperature(int ct) {
            var command = new LightCommand { ColorTemperature = ct };
            var result = await Hue.SendCommandAsync(command, new List<string> { LocalId });
            if (result[0].Error == null) {
                ColorTemperature = ct;
                NotifyObservers(nameof(ColorTemperature), ColorTemperature);
            }
        }

    }

}