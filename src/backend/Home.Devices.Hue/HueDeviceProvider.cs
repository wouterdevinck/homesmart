using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Home.Core;
using Home.Core.Configuration;
using Home.Core.Configuration.Models;
using Home.Core.Interfaces;
using Home.Devices.Hue.Devices;
using Microsoft.Extensions.Logging;
using Q42.HueApi;

namespace Home.Devices.Hue {

    public class HueDeviceProvider : AbstractDeviceProvider {

        public static ProviderDescription Descriptor = new("hue", ProviderDescriptionType.DeviceProvider, typeof(HueDeviceProvider), typeof(HueConfiguration));

        private readonly List<DeviceConfigurationModel> _models;
        private readonly ILogger _logger;
        private readonly HueConfiguration _configuration; 
        private readonly List<IDevice> _devices;
        private HueClient _hue;
        private Timer _timer;
        private DateTime _backOffTime;

        private const string TypePlug = "On/Off plug-in unit";
        private const string TypeDimmable = "Dimmable light";
        private const string TypeColorTemperature = "Color temperature light";

        public HueDeviceProvider(List<DeviceConfigurationModel> models, ILogger logger, IDeviceProviderConfiguration configuration) {
            _models = models;
            _logger = logger;
            _configuration = configuration as HueConfiguration;
            _devices = new List<IDevice>();
        }

        public override async Task ConnectAsync() {

            _logger.LogInformation($"Connecting");

            // Discover bridges
            var locator = new HttpBridgeLocator();
            var bridges = (await locator.LocateBridgesAsync(TimeSpan.FromMinutes(1))).ToList();
            if (bridges.Count == 0) throw new Exception("No bridges found");
            if (bridges.Count > 1) throw new Exception("Multiple bridges found, not supported");
            var bridgeInfo = bridges.Single();

            // Connect to bridge
            _hue = new LocalHueClient(bridgeInfo.IpAddress);
            _hue.Initialize(_configuration.ApiKey);

            // Get all devices from the bridge
            _devices.AddRange(await GetAllDevices());

            // Notify for each device
            NotifyObservers(_devices);

            // Periodically check if new devices have been added
            _backOffTime = DateTime.Now + TimeSpan.FromMilliseconds(_configuration.Polling.BackOffInterval);
            _timer = new Timer(_configuration.Polling.MinPollInterval);
            _timer.Elapsed += OnUpdate;
            _timer.AutoReset = true;
            _timer.Enabled = true;

        }

        public override Task DisconnectAsync() {
            _logger.LogInformation("Disconnecting");
            _timer.Enabled = false;
            _timer.Dispose();
            _devices.Clear();
            _hue = null;
            return Task.CompletedTask;
        }

        public override IEnumerable<IDevice> GetDevices() {
            return _devices;
        }

        private async Task<List<IDevice>> GetAllDevices() {
            var devices = new List<IDevice>();

            // Bridges
            var bridge = await _hue.GetBridgeAsync();
            devices.Add(new HueBridgeDevice(bridge, _hue));

            // Lights and plugs
            var lights = await _hue.GetLightsAsync();
            devices.AddRange(lights.Select(x => (IDevice)(x.Type switch {
                TypePlug => new HuePlugDevice(x, _hue),
                TypeDimmable => new HueDimmableLightDevice(x, _hue),
                TypeColorTemperature => new HueColorTemperatureLightDevice(x, _hue),
                _ => null
            })).Where(x => x is not null));

            // Switches
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            var sensors = (await _hue.GetSensorsAsync()).Where(x => x.Capabilities != null);
            devices.AddRange(sensors.Select(x => new HueSwitchDevice(x, _hue) as IDevice));

            return devices;
        }

        private async void OnUpdate(object source, ElapsedEventArgs e) {

            // Back off exponentially
            if (DateTime.Now >= _backOffTime) {
                _timer.Interval = Math.Min(_timer.Interval * _configuration.Polling.BackOffFactor, _configuration.Polling.MaxPollInterval);
                _backOffTime = DateTime.Now + TimeSpan.FromMilliseconds(_configuration.Polling.BackOffInterval);
            }

            // Get all devices and calculate the delta
            var allDevices = await GetAllDevices();
            var devices = _devices.ToList();
            var newDevices = allDevices.Where(x => devices.All(y => y.DeviceId != x.DeviceId)).ToList();
            var existingDevices = allDevices.Where(x => devices.Any(y => y.DeviceId == x.DeviceId)).ToList();
            var updatedDevices = existingDevices.Where(x => !x.Equals(devices.Single(y => y.DeviceId == x.DeviceId))).ToList();

            // Update the repository
            _devices.AddRange(newDevices);
            foreach (var device in updatedDevices) {
                _devices.Single(x => x.DeviceId == device.DeviceId).Update(device);
            }

            // Notify for new devices
            NotifyObservers(newDevices);

            // In case there were any updates, go back to faster polling
            if (updatedDevices.Any() || newDevices.Any()) {
                FastUpdate();
            }

            // Log some info
            _logger.LogInformation($"Polled Hue bridge at {DateTime.Now}. " +
                $"Total of {allDevices.Count} devices, {newDevices.Count} new and {updatedDevices.Count} updated. " +
                $"Next poll in {_timer.Interval}ms. ");

        }

        private void FastUpdate() {
            _timer.Interval = _configuration.Polling.MinPollInterval;
            _backOffTime = DateTime.Now + TimeSpan.FromMilliseconds(_configuration.Polling.BackOffInterval);
        }

    }

}
