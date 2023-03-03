using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Home.Core.Configuration;
using Home.Core.Interfaces;
using Home.Core.Models;
using Microsoft.Extensions.Logging;

namespace Home.Core {

    public class DeviceProviderCollection : AbstractDeviceProvider, ITelemetryProvider {

        private readonly List<IDeviceProvider> _providers;
        private readonly ILogger<DeviceProviderCollection> _logger;

        public DeviceProviderCollection(ConfigurationReader config, ILoggerFactory loggerFactory) : base(config.ConfigurationModel.Home) {
            _providers = new List<IDeviceProvider>();
            _logger = loggerFactory.CreateLogger<DeviceProviderCollection>();

            // Consistency check
            if (!config.ConfigurationModel.Home.CheckConsistency()) { 
                throw new Exception("Config consistency error!");
            }

            // Device providers
            foreach (var provider in config.ConfigurationModel.DeviceProviders) {
                var implType = config.ProviderDescriptors.SingleOrDefault(x => x.Tag == provider.Key && x.Type == DescriptorType.DeviceProvider).ProviderType;
                if (implType == null) {
                    _logger.LogError("Failed to add device provider: type not found");
                    continue;
                }
                var logger = loggerFactory.CreateLogger(implType);
                var impl = Activator.CreateInstance(implType, config.ConfigurationModel.Home, logger, provider.Value) as IDeviceProvider;
                if (impl == null) {
                    _logger.LogError("Failed to add device provider: implementation could not be constructed");
                    continue;
                }
                _logger.LogInformation($"Adding device provider: {provider.Key}");
                AddProvider(impl);
            }

            // Device consumers
            if (config.ConfigurationModel.DeviceConsumers != null) {
                foreach (var consumer in config.ConfigurationModel.DeviceConsumers) {
                    var tag = consumer.Keys.SingleOrDefault();
                    var model = consumer.SingleOrDefault().Value;
                    if (tag == null || model == null) {
                        _logger.LogError("Failed to add device consumer: tag or model could not be read");
                        continue;
                    }
                    var implType = config.ProviderDescriptors.SingleOrDefault(x => x.Tag == tag && x.Type == DescriptorType.DeviceConsumer).ProviderType;
                    if (implType == null) {
                        _logger.LogError("Failed to add device consumer: type not found");
                        continue;
                    }
                    var logger = loggerFactory.CreateLogger(implType);
                    var impl = Activator.CreateInstance(implType, logger, model.Configuration) as IDeviceConsumer;
                    if (impl == null) {
                        _logger.LogError("Failed to add device consumer: implementation could not be constructed");
                        continue;
                    }
                    _logger.LogInformation($"Adding device consumer: {tag} - {model.Description}");
                    InstallDeviceConsumer(model.Description, impl);
                }
            }

        }

        public void AddProvider(IDeviceProvider provider) {
            _providers.Add(provider);
            provider.DeviceDiscovered += (_, d) => NotifyObservers(d);
            NotifyObservers(provider.GetDevices());
        }

        public override Task ConnectAsync() {
            _logger.LogInformation($"Connecting all device providers");
            return Task.WhenAll(_providers.Select(provider => provider.ConnectAsync()));
        }

        public override Task DisconnectAsync() {
            _logger.LogInformation($"Disconnecting all device providers");
            return Task.WhenAll(_providers.Select(provider => provider.DisconnectAsync()));
        }

        public override IEnumerable<IDevice> GetDevices() {
            return _providers.Aggregate(Enumerable.Empty<IDevice>(),
                (current, provider) => current.Concat(provider.GetDevices()));
        }

        public async Task<IEnumerable<IDataPoint>> GetDataAsync(string device, string point, TimeRange range) {
            var consumers = GetDeviceConsumers();
            foreach (var consumer in consumers) {
                var tp = consumer as ITelemetryProvider;
                if (tp != null) {
                    return await tp.GetDataAsync(device, point, range);
                }
            }
            return null;
        }

    }

}