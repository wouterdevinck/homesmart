using Home.Core;
using Home.Core.Attributes;
using Home.Core.Configuration.Models;

namespace Home.Devices.Somfy {

    [Device]
    public abstract partial class SomfyDevice : AbstractDevice {

        protected SomfyDevice(HomeConfigurationModel home, string id) : base(home, id) {}

        public void UpdateAvailability(bool available) {
            if (Reachable != available) {
                Reachable = available;
                NotifyObservers(nameof(Reachable), Reachable);
            }
        }

    }

}
