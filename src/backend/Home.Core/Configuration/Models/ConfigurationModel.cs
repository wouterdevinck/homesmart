using System.Collections.Generic;
using Home.Core.Configuration.Interfaces;

namespace Home.Core.Configuration.Models {

    public class ConfigurationModel {

        public GlobalConfigurationModel Global { get; set; }
        public HomeConfigurationModel Home { get; set; }

        public Dictionary<string, IProviderConfiguration> DeviceProviders { get; set; }
        public Dictionary<string, ITelemetryConfiguration> Telemetry { get; set; }
        public Dictionary<string, IRemoteConfiguration> Remote { get; set; }

        public List<Dictionary<string, ConfigurationWithDescriptionModel<IAutomationConfiguration>>> Automations { get; set; }

    }

}