using System;
using System.Threading.Tasks;
using Home.Core;
using Home.Core.Attributes;
using Home.Core.Configuration.Models;
using Home.Core.Devices;
using Home.Devices.HomeWizard.Models;

namespace Home.Devices.HomeWizard.Devices {

    [Device]
    public partial class HomeWizardWaterMeterDevice : HomeWizardDevice, IWatermeter {

        [DeviceProperty]
        public int TotalLiters { get; private set; }

        [DeviceProperty]
        public double LitersPerMinute { get; private set;  }

        public HomeWizardWaterMeterDevice(HomeConfigurationModel home, HomeWizardApiClient api, DeviceModel device) : base(home, api, device, $"HW-WATER-{device.Serial}") {
            Type = Helpers.GetTypeString(Helpers.DeviceType.Watermeter);
            Poll().Wait();
        }

        public sealed override async Task Poll() {
            var reachable = true;
            try {
                var data = await Api.GetData<WaterDataModel>();
                if (TotalLiters != data.TotalLiters) {
                    TotalLiters = data.TotalLiters;
                    NotifyObservers(nameof(TotalLiters), TotalLiters);
                }
                if (Math.Abs(LitersPerMinute - data.ActiveLiterLpm) >= Tolerance) {
                    LitersPerMinute = data.ActiveLiterLpm;
                    NotifyObservers(nameof(LitersPerMinute), LitersPerMinute);
                }
            } catch (Exception) {
                reachable = false;
            }
            if (Reachable != reachable) {
                Reachable = reachable;
                NotifyObservers(nameof(Reachable), Reachable);
            }
        }

    }

}