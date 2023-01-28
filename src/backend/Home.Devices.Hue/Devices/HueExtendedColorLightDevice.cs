using System.Collections.Generic;
using System.Threading.Tasks;
using Home.Core.Attributes;
using Home.Core.Devices;
using Home.Core.Models;
using Q42.HueApi;

namespace Home.Devices.Hue.Devices {

    [Device]
    public partial class HueExtendedColorLightDevice : HueColorTemperatureLightDevice, IColorLight {
        
        public HueExtendedColorLightDevice(Light light, HueClient hue) : base(light, hue) {
            if (light.State.ColorCoordinates != null) {
                ColorXy = new ColorXy {
                    X = light.State.ColorCoordinates[0],
                    Y = light.State.ColorCoordinates[1]
                };
            }
        }

        [DeviceProperty]
        public ColorXy ColorXy { get; private set; }

        public async Task SetColorXy(ColorXy c) {
            var command = new LightCommand {
                ColorCoordinates = new[] { c.X, c.Y }
            };
            var result = await Hue.SendCommandAsync(command, new List<string> { LocalId });
            if (result[0].Error == null) {
                ColorXy = c;
                NotifyObservers(nameof(ColorTemperature), ColorTemperature);
            }
        }

    }

}