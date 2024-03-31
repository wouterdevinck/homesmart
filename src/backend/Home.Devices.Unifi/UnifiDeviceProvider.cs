using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Home.Core;
using Home.Core.Configuration;
using Home.Core.Configuration.Interfaces;
using Home.Core.Configuration.Models;
using Home.Core.Interfaces;
using Home.Devices.Unifi.Devices;
using Home.Devices.Unifi.Models;
using Microsoft.Extensions.Logging;

namespace Home.Devices.Unifi {

    public class UnifiDeviceProvider : AbstractDeviceProvider, IDisposable {

        public static Descriptor Descriptor = new("unifi", typeof(UnifiDeviceProvider), typeof(UnifiConfiguration), DescriptorType.Provider);

        private readonly List<UnifiDevice> _devices;
        private readonly HomeConfigurationModel _home;
        private readonly ILogger _logger;
        private readonly UnifiConfiguration _configuration;
        private readonly UnifiApiClient _api;

        public UnifiDeviceProvider(HomeConfigurationModel home, ILogger logger, IProviderConfiguration configuration) {
            _devices = new List<UnifiDevice>();
            _home = home;
            _logger = logger;
            _configuration = (UnifiConfiguration)configuration;
            _api = new UnifiApiClient(_configuration.Ip, _configuration.Site, _configuration.Username, _configuration.Password);
        }

        private UnifiDevice NetworkDeviceFactory(DeviceModel model) {
            return model.Model switch {
                "UDMPRO" => new UnifiConsole(_home, model),
                "USMINI" => new UnifiNetworkSwitch(_home, model),
                "USM8P60" => new UnifiNetworkSwitch(_home, model),
                "UAL6" => new UnifiAccessPoint(_home, model),
                _ => null
            };
        }

        public override async Task ConnectAsync() {
            if (_configuration == null) {
                throw new Exception("Configuration missing");
            }
            await TryConnectAsync();
        }

        private async Task TryConnectAsync() {
            _logger.LogInformation($"Connecting to site {_configuration.Site}");
            try {
                var devices = await _api.GetDevicesAsync();
                _devices.AddRange(devices.NetworkDevices.Select(NetworkDeviceFactory));
                _devices.AddRange(devices.ProtectDevices.Select(x => new UnifiCamera(_home, x)));
                NotifyObservers(_devices);
            } catch (Exception ex) {
                _logger.LogError($"Failed to connect with reason - {ex.Message}");
            }
        }

        public override Task DisconnectAsync() {
            _logger.LogInformation("Disconnecting");
            return Task.CompletedTask;
        }

        public override IEnumerable<IDevice> GetDevices() {
            return _devices;
        }

        public void Dispose() {
            _api.Dispose();
        }

    }

}