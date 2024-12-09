using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Home.Core;
using Home.Core.Configuration;
using Home.Core.Configuration.Interfaces;
using Home.Core.Configuration.Models;
using Home.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace Home.Devices.Meta {

    // TODO add to frontend

    public class MetaDeviceProvider : AbstractDeviceProvider {

        public static Descriptor Descriptor = new("meta", typeof(MetaDeviceProvider), null, DescriptorType.Provider);

        private readonly List<MetaHubDevice> _devices;
        private readonly HomeConfigurationModel _home;
        private readonly ILogger _logger;
        private readonly EdgeOsApiClient _api;

        public MetaDeviceProvider(HomeConfigurationModel home, ILogger logger, IProviderConfiguration _) {
            _devices = [];
            _home = home;
            _logger = logger;
            _api = new EdgeOsApiClient();
        }

        public override async Task ConnectAsync() {
            _logger.LogInformation("Connecting");
            try {
                var status = await _api.GetStatus();
                _devices.Add(new MetaHubDevice(_home, _api, status));
                NotifyObservers(_devices);
            } catch (Exception ex) {
                _logger.LogError($"Connect error - {ex.Message}");
            }
        }

        public override Task DisconnectAsync() {
            _logger.LogInformation("Disconnecting");
            return Task.CompletedTask;
        }

        public override IEnumerable<IDevice> GetDevices() {
            return _devices;
        }

    }

}