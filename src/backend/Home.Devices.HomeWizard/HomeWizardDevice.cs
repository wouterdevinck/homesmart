using System.Threading.Tasks;
using Home.Core;
using Home.Core.Attributes;
using Home.Core.Configuration.Models;
using Home.Core.Interfaces;
using Home.Devices.HomeWizard.Models;

namespace Home.Devices.HomeWizard {

    [Device]
    public abstract partial class HomeWizardDevice : AbstractDevice, IDevice {

        protected readonly HomeWizardApiClient Api;

        protected HomeWizardDevice(HomeConfigurationModel home, HomeWizardApiClient api, DeviceModel device, string id) : base(home, id) {
            Api = api;
            Name = device.ProductName;
            Manufacturer = Helpers.HomeWizard;
            Version = device.FirmwareVersion;
            Model = device.ProductType;
            Reachable = true;
        }

        public abstract Task Poll();

    }

}