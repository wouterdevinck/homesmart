using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Home.Core;
using Home.Core.Attributes;
using Home.Core.Configuration.Models;
using Home.Core.Devices;
using Home.Core.Models;
using Home.Devices.Tuya.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Home.Devices.Tuya.Devices {

    [Device]
    public partial class TuyaFancoilDevice : TuyaDevice, IFancoil {

        private readonly Dictionary<FancoilMode, string> _fancoilModes = new() {
            { FancoilMode.Heating, "heat" },
            { FancoilMode.Cooling, "cold" }
        };

        private readonly Dictionary<FancoilSpeed, string> _fancoilSpeeds = new() {
            { FancoilSpeed.Auto,   "auto"   },
            { FancoilSpeed.Low,    "low"    },
            { FancoilSpeed.Medium, "middle" },
            { FancoilSpeed.High,   "high"   }
        };

        protected TuyaConfiguration Configuration;

        public TuyaFancoilDevice(HomeConfigurationModel home, ILogger logger, TuyaDeviceConfiguration model, TuyaConfiguration configuration) : base(home, logger, model) {
            Configuration = configuration;
            Name = model.Name;
            Manufacturer = Helpers.Jaga.HarmonizeManufacturer();
            Version = Helpers.VersionNotAvailable;
            Model = "JRT-100TW";
            Type = Helpers.GetTypeString(Helpers.DeviceType.Fancoil);
            Reachable = false;
        }

        [DeviceProperty]
        public bool On { get; private set; }

        [DeviceProperty(Unit = "\u00b0C")]
        public double Temperature { get; private set; }

        [DeviceProperty(Unit = "\u00b0C")]
        public double RequestedTemperature { get; private set; }

        [DeviceProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        public FancoilMode Mode { get; private set; }

        [DeviceProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        public FancoilSpeed Speed { get; private set; }

        [DeviceCommand]
        public async Task ToggleOnOffAsync() {
            if (On) {
                await TurnOffAsync();
            } else {
                await TurnOnAsync();
            }
        }

        [DeviceCommand]
        public async Task TurnOnAsync() {
            await SetDpAsync(1, true);
        }

        [DeviceCommand]
        public async Task TurnOffAsync() {
            await SetDpAsync(1, false);
        }

        [DeviceCommand]
        public async Task SetRequestedTemperatureAsync(double t) {
            await SetDpAsync(2, (int)(t * 10));
        }

        [DeviceCommand]
        public async Task SetModeAsync(FancoilMode m) {
            await SetDpAsync(4, _fancoilModes[m]);
        }

        [DeviceCommand]
        public async Task SetSpeedAsync(FancoilSpeed s) {
            await SetDpAsync(5, _fancoilSpeeds[s]);
        }

        protected override void ProcessDps(TuyaDps dps) {
            foreach (var dp in dps.Dps) {
                switch (dp.Key) {
                    case 1:
                        var on = (bool)dp.Value;
                        if (On != on) {
                            On = on;
                            NotifyObservers(nameof(On), On);
                        }
                        break;
                    case 2:
                        RequestedTemperature = (double)(long)dp.Value / 10;
                        NotifyObservers(nameof(RequestedTemperature), RequestedTemperature);
                        break;
                    case 3:
                        Temperature = (double)(long)dp.Value / 10;
                        NotifyObservers(nameof(Temperature), Temperature);
                        break;
                    case 4:
                        var mode = _fancoilModes.SingleOrDefault(x => x.Value == (string)dp.Value).Key;
                        if (Mode != mode) {
                            Mode = mode;
                            NotifyObservers(nameof(Mode), Mode);
                        }
                        break;
                    case 5:
                        var speed = _fancoilSpeeds.SingleOrDefault(x => x.Value == (string)dp.Value).Key;
                        if (Speed != speed) {
                            Speed = speed;
                            NotifyObservers(nameof(Speed), Speed);
                        }
                        break;
                }
            }
        }

        protected override void UpdateAvailability(bool available) {
            if (Reachable != available) {
                Reachable = available;
                NotifyObservers(nameof(Reachable), Reachable);
            }
        }

    }

}