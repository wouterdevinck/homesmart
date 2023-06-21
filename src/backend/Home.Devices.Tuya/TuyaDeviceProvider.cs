using System.Collections.Generic;
using System.Threading.Tasks;
using Home.Core;
using Home.Core.Configuration;
using Home.Core.Configuration.Interfaces;
using Home.Core.Configuration.Models;
using Home.Core.Interfaces;
using Home.Devices.Tuya.Devices;
using Microsoft.Extensions.Logging;

namespace Home.Devices.Tuya {

    public class TuyaDeviceProvider : AbstractDeviceProvider {

        public static Descriptor Descriptor = new("tuya", typeof(TuyaDeviceProvider), typeof(TuyaConfiguration), DescriptorType.Provider);

        private readonly HomeConfigurationModel _home;
        private readonly ILogger _logger;
        private readonly TuyaConfiguration _configuration; 
        private readonly List<TuyaDevice> _devices;

        public TuyaDeviceProvider(HomeConfigurationModel home, ILogger logger, IProviderConfiguration configuration) {
            _home = home;
            _logger = logger;
            _configuration = configuration as TuyaConfiguration;
            _devices = new List<TuyaDevice>();
        }

        private TuyaDevice DeviceFactory(TuyaDeviceConfiguration deviceConfig) {
            return deviceConfig.Type switch {
                "fancoil" => new TuyaFancoilDevice(_home, _logger, deviceConfig, _configuration),
                _ => null
            };
        }

        public override async Task ConnectAsync() {
            _logger.LogInformation($"Connecting");
            foreach (var deviceConfig in _configuration.Devices) {
                var device = DeviceFactory(deviceConfig);
                await device.ConnectAsync();
                _devices.Add(device);
            }
            NotifyObservers(_devices);
        }

        public override async Task DisconnectAsync() {
            _logger.LogInformation("Disconnecting");
            foreach(var device in _devices) {
                await device.DisconnectAsync();
            }
            _devices.Clear();
        }

        public override IEnumerable<IDevice> GetDevices() {
            return _devices;
        }

    }

}


