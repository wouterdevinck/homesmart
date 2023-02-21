using System.Collections.Generic;
using System.Threading.Tasks;
using Home.Core;
using Home.Core.Attributes;
using Home.Core.Configuration.Models;
using Home.Core.Devices;
using Q42.HueApi;

namespace Home.Devices.Hue.Common {

    [Device]
    public abstract partial class HueOnOffDevice : HueDevice, IOnOffDevice {

        protected HueOnOffDevice(HueClient hue, Light light, HomeConfigurationModel home) : base(hue, light.Id, home, $"HUE-LIGHT-{light.UniqueId}") {
            Name = light.Name;
            if (light.State.IsReachable != null) Reachable = light.State.IsReachable.Value;
            On = light.State.On && light.State.IsReachable == true;
            Manufacturer = light.ManufacturerName.HarmonizeManufacturer();
            Model = light.ModelId.HarmonizeModel();
            Version = light.SoftwareVersion;
            if (string.IsNullOrEmpty(Version)) Version = Helpers.VersionNotAvailable;
        }

        [DeviceProperty]
        public bool On { get; protected set; }

        public async Task TurnOnAsync() {
            var command = new LightCommand { On = true };
            var result = await Hue.SendCommandAsync(command, new List<string> { LocalId });
            if (result[0].Error is null) {
                On = true;
                NotifyObservers(nameof(On), On);
            }
            // TODO Return result?
        }

        public async Task TurnOffAsync() {
            var command = new LightCommand { On = false };
            var result = await Hue.SendCommandAsync(command, new List<string> { LocalId });
            if (result[0].Error is null) {
                On = false;
                NotifyObservers(nameof(On), On);
            }
            // TODO Return result?
        }

        public Task ToggleOnOffAsync() {
            if (On) { 
                return TurnOffAsync();
            }
            return TurnOnAsync();
        }

    }

}