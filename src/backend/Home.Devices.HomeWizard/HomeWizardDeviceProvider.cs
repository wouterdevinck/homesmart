using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Home.Core;
using Home.Core.Configuration;
using Home.Core.Configuration.Interfaces;
using Home.Core.Configuration.Models;
using Home.Core.Interfaces;
using Home.Devices.HomeWizard.Devices;
using Microsoft.Extensions.Logging;

namespace Home.Devices.HomeWizard {

    public class HomeWizardDeviceProvider : AbstractDeviceProvider {

        public static Descriptor Descriptor = new("homewizard", typeof(HomeWizardDeviceProvider), typeof(HomeWizardConfiguration), DescriptorType.Provider);

        private readonly List<HomeWizardDevice> _devices;
        private readonly HomeConfigurationModel _home;
        private readonly ILogger _logger;
        private readonly HomeWizardConfiguration _config;
        private readonly Timer _timer;

        public HomeWizardDeviceProvider(HomeConfigurationModel home, ILogger logger, IProviderConfiguration config) {
            _devices = [];
            _home = home;
            _logger = logger;
            _config = config as HomeWizardConfiguration;
            if (_config != null) {
                _timer = new Timer(_config.PollingInterval * 1000);
                _timer.Elapsed += OnUpdate;
                _timer.AutoReset = true;
                _timer.Enabled = true;
            }
        }

        public override async Task ConnectAsync() {
            _logger.LogInformation("Connecting");
            try {
                foreach (var api in _config.Devices.Select(x => x.Ip).Select(ip => new HomeWizardApiClient(ip))) {
                    var status = await api.GetStatus();
                    if (status.ProductName == "Watermeter") {
                        _devices.Add(new HomeWizardWaterMeterDevice(_home, api, status));
                    }
                }
                NotifyObservers(_devices);
            } catch (Exception ex) {
                _logger.LogError($"Connect error - {ex.Message}");
            }
        }

        public override Task DisconnectAsync() {
            _logger.LogInformation("Disconnecting");
            _timer.Enabled = false;
            _timer.Dispose();
            _devices.Clear();
            return Task.CompletedTask;
        }

        public override IEnumerable<IDevice> GetDevices() {
            return _devices;
        }

        private async void OnUpdate(object source, ElapsedEventArgs e) {
            foreach (var device in _devices) {
                await device.Poll();
            }
        }

    }

}