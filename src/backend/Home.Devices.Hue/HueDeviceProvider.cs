using System.Collections.Generic;
using System.Threading.Tasks;
using Home.Core;
using Home.Core.Configuration;
using Home.Core.Configuration.Models;
using Home.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace Home.Devices.Hue {

    public class HueDeviceProvider : AbstractDeviceProvider {

        public static ProviderDescription Descriptor = new("hue", ProviderDescriptionType.DeviceProvider, typeof(HueDeviceProvider), typeof(HueConfiguration));

        private readonly List<DeviceConfigurationModel> _models;
        private readonly ILogger _logger;
        private readonly HueConfiguration _configuration; 
        private readonly List<IDevice> _devices;

        public HueDeviceProvider(List<DeviceConfigurationModel> models, ILogger logger, IDeviceProviderConfiguration configuration) {
            _models = models;
            _logger = logger;
            _configuration = configuration as HueConfiguration;
            _devices = new List<IDevice>();
        }

        public override async Task ConnectAsync() {
            _logger.LogInformation($"Connecting");

        }

        public override async Task DisconnectAsync() {
            _logger.LogInformation("Disconnecting");
      
        }

        public override IEnumerable<IDevice> GetDevices() {
            return _devices;
        }

    }

}
