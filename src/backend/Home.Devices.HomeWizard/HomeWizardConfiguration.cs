using System.Collections.Generic;
using Home.Core.Configuration.Interfaces;

namespace Home.Devices.HomeWizard {

    public class HomeWizardConfiguration : IProviderConfiguration {

        public uint PollingInterval { get; set; }
        public List<HomeWizardDeviceConfiguration> Devices { get; set; }

    }

    public class HomeWizardDeviceConfiguration {

        public string Ip { get; set; }

    }

}
