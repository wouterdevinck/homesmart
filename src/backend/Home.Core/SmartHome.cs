using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Home.Core.Attributes;
using Home.Core.Configuration;
using Home.Core.Configuration.Interfaces;
using Home.Core.Configuration.Models;
using Home.Core.Interfaces;
using Home.Core.Models;
using Microsoft.Extensions.Logging;

namespace Home.Core {

    public class SmartHome : ISmartHome {

        public event EventHandler<IDevice> DeviceDiscovered;

        private readonly ILogger<SmartHome> _logger;
        private readonly ConfigurationModel _configuration;
        private readonly List<IDeviceProvider> _providers = [];
        private readonly List<IAutomation> _automations = [];
        private ITelemetry _telemetry;
        private IRemote _remote;

        public SmartHome(ConfigurationReader config, ILoggerFactory loggerFactory) {
            _logger = loggerFactory.CreateLogger<SmartHome>();
            _configuration = config.ConfigurationModel;

            // Consistency check
            if (config.ConfigurationModel.Home == null) {
                config.ConfigurationModel.Home = new HomeConfigurationModel() {
                    Devices = [],
                    Rooms = []
                };
            } else if (!config.ConfigurationModel.Home.CheckConsistency()) { 
                throw new Exception("Config consistency error!");
            }

            // Make sure there's a list of device providers
            _configuration.DeviceProviders ??= new Dictionary<string, IProviderConfiguration>();

            // Make sure the meta device provider is added
            _configuration.DeviceProviders.TryAdd("meta", null);

            // Device providers
            if (_configuration.DeviceProviders is { Count: > 0 }) {
                foreach (var provider in _configuration.DeviceProviders) {
                    Add<IDeviceProvider, IProviderConfiguration>(
                        "device provider",
                        provider.Key,
                        provider.Value,
                        config.ProviderDescriptors,
                        DescriptorType.Provider,
                        loggerFactory,
                        impl => {
                            _providers.Add(impl);
                            impl.DeviceDiscovered += (_, d) => DeviceDiscovered?.Invoke(this, d);
                        },
                        (l, c) => [_configuration.Home, l, c]
                    );
                }
            } else {
                _logger.LogError("No device providers loaded. Check your configuration");
            }

            // Automation
            if (_configuration.Automations is { Count: > 0 }) {
                foreach (var consumer in _configuration.Automations) {
                    var model = consumer.SingleOrDefault().Value;
                    Add<IAutomation, IAutomationConfiguration> (
                        "automation",
                        consumer.Keys.SingleOrDefault(),
                        model.Configuration,
                        config.ProviderDescriptors,
                        DescriptorType.Automation,
                        loggerFactory,
                        impl => {
                            impl.Description = model.Description;
                            _automations.Add(impl);
                            impl.Install(this);
                        },
                        (l, c) => [l, c]
                    );
                }
            }

            // Telemetry
            if (_configuration.Telemetry is { Count: > 0 }) {
                if (_configuration.Telemetry.Count != 1) {
                    _logger.LogError("Telemetry: multiple entries, first will be used");
                }
                var telemetry = _configuration.Telemetry.First();
                Add<ITelemetry, ITelemetryConfiguration>(
                    "telemetry",
                    telemetry.Key,
                    telemetry.Value,
                    config.ProviderDescriptors,
                    DescriptorType.Telemetry,
                    loggerFactory,
                    impl => {
                        _telemetry = impl;
                        impl.Install(this);
                    },
                    (l, c) => [l, c, _configuration.Global]
                );
            }

            // Remote
            if (_configuration.Remote is { Count: > 0 }) {
                if (_configuration.Remote.Count != 1) {
                    _logger.LogError("Remote: multiple entries, first will be used");
                }
                var remote = _configuration.Remote.First();
                Add<IRemote, IRemoteConfiguration>(
                    "remote",
                    remote.Key,
                    remote.Value,
                    config.ProviderDescriptors,
                    DescriptorType.Remote,
                    loggerFactory,
                    impl => {
                        _remote = impl;
                        impl.Install(this);
                    },
                    (l, c) => [l, c]
                );
            }

        }

        private void Add<T, U>(string logTag, string tag, U config, List<Descriptor> providers, DescriptorType type, ILoggerFactory loggerFactory, Action<T> process, Func<ILogger, U, object[]> args) {
            if (tag == null) {
                _logger.LogError($"Failed to add {logTag}: tag could not be read");
                return;
            }
            var implType = providers.SingleOrDefault(x => x.Tag == tag && x.Type == type)?.ProviderType;
            if (implType == null) {
                _logger.LogError($"Failed to add {logTag}: type not found");
                return;
            }
            var logger = loggerFactory.CreateLogger(implType);
            if (Activator.CreateInstance(implType, args(logger, config)) is not T impl) {
                _logger.LogError($"Failed to add {logTag}: implementation could not be constructed");
                return;
            }
            _logger.LogInformation($"Adding {logTag}: {tag}");
            process(impl);
        }

        public Task ConnectAsync() {
            _logger.LogInformation("Connecting all device providers");
            return Task.WhenAll(_providers.Select(provider => provider.ConnectAsync()));
        }

        public Task DisconnectAsync() {
            _logger.LogInformation("Disconnecting all device providers");
            return Task.WhenAll(_providers.Select(provider => provider.DisconnectAsync()));
        }

        public IEnumerable<IDevice> GetDevices() {
            return _providers.Aggregate(Enumerable.Empty<IDevice>(),
                (current, provider) => current.Concat(provider.GetDevices()));
        }

        public IEnumerable<RoomConfigurationModel> GetRooms() {
            return _configuration.Home.Rooms;
        }

        public IEnumerable<IDeviceProvider> GetProviders() {
            return _providers;
        }

        public IEnumerable<IAutomation> GetAutomations() {
            return _automations;
        }

        public ITelemetry GetTelemetry() {
            return _telemetry;
        }

        public IRemote GetRemote() {
            return _remote;
        }

        public async Task<DataSet> GetData(string deviceId, string point, string since, string toAgo, DateTime? from, DateTime? to, string meanWindow, string diffWindow) {
            if (string.IsNullOrEmpty(since)) since = "24h";
            TimeRange range;
            IEnumerable<IDataPoint> points;
            if (from != null) {
                if (to != null) {
                    range = new TimeRange(from.Value, to.Value);
                } else {
                    range = new TimeRange(from.Value);
                }
            } else if (string.IsNullOrEmpty(toAgo)) {
                range = new TimeRange(new RelativeTime(since));
            } else {
                range = new TimeRange(new RelativeTime(since), new RelativeTime(toAgo));
            }
            if (!string.IsNullOrEmpty(diffWindow)) {
                var window = new RelativeTime(diffWindow);
                var tmp = await _telemetry.GetWindowDifference(deviceId, point, range, window);
                points = tmp.Select(x => new LabeledDataPoint(x, window, _configuration.Global));
            } else if (!string.IsNullOrEmpty(meanWindow)) {
                var window = new RelativeTime(meanWindow);
                var tmp = await _telemetry.GetWindowMean(deviceId, point, range, window);
                points = tmp.Select(x => new LabeledDataPoint(x, window, _configuration.Global));
            } else {
                points = await _telemetry.GetAllData(deviceId, point, range);
            }
            var unit = GetDevices().SingleOrDefault(x => x.HasId(deviceId))?.GetPropertyInfo(point)?.Unit ?? string.Empty;
            return new DataSet(deviceId, point, unit, points);
        }

    }

}