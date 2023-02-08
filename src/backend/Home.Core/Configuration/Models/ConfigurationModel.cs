using System.Collections.Generic;
using Home.Core.Configuration.Interfaces;

namespace Home.Core.Configuration.Models {

    public class ConfigurationModel {

        public Dictionary<string, IDeviceProviderConfiguration> DeviceProviders { get; set; }
        public List<DeviceConfigurationModel> Devices { get; set; }
        public List<Dictionary<string, ConfigurationWithDescriptionModel<IDeviceConsumerConfiguration>>> DeviceConsumers { get; set; }

    }

}
