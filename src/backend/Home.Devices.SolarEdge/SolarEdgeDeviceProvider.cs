using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Home.Core;
using Home.Core.Configuration;
using Home.Core.Configuration.Interfaces;
using Home.Core.Configuration.Models;
using Home.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace Home.Devices.SolarEdge {

    public class SolarEdgeDeviceProvider : AbstractDeviceProvider, IDisposable {

        public static Descriptor Descriptor = new("solaredge", typeof(SolarEdgeDeviceProvider), typeof(SolarEdgeConfiguration), DescriptorType.Provider);

        private readonly List<SolarEdgeInverterDevice> _devices;
        private readonly HomeConfigurationModel _home;
        private readonly ILogger _logger;
        private readonly SolarEdgeConfiguration _configuration;
        private readonly SolarEdgeApiClient _api;

        public SolarEdgeDeviceProvider(HomeConfigurationModel home, ILogger logger, IProviderConfiguration configuration) {
            _devices = new List<SolarEdgeInverterDevice>();
            _home = home;
            _logger = logger;
            _configuration = (SolarEdgeConfiguration)configuration;
            _api = new SolarEdgeApiClient(_configuration.Site, _configuration.ApiKey);
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
                var inverter = await _api.GetInverter();
                _devices.Add(new SolarEdgeInverterDevice(_home, _logger, _api, inverter));
                NotifyObservers(_devices);
            } catch (Exception ex) {
                _logger.LogError($"Failed to connect with reason - {ex.Message}");
            }
        }

        public override Task DisconnectAsync() {
            _logger.LogInformation("Disconnecting");
            foreach(var device in _devices) {
                device.Dispose();
            }
            return Task.CompletedTask;
        }

        public override IEnumerable<IDevice> GetDevices() {
            return _devices;
        }

        public void Dispose() {
            foreach (var device in _devices) {
                device.Dispose();
            }
            _api.Dispose();
        }

    }

}