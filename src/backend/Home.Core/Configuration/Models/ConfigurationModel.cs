using System.Collections.Generic;

namespace Home.Core.Configuration.Models {

    public class ConfigurationModel {

        public Dictionary<string, IDeviceProviderConfiguration> DeviceProviders { get; set; }
        public List<DeviceConfigurationModel> Devices { get; set; }
        public List<Dictionary<string, AutomationConfigurationModel>> Automations { get; set; }

    }

}
