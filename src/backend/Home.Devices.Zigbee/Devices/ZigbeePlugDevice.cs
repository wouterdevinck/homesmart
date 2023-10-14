using System.Threading.Tasks;
using Home.Core;
using Home.Core.Attributes;
using Home.Core.Configuration.Models;
using Home.Core.Devices;
using Home.Devices.Zigbee.Models;
using Home.Devices.Zigbee.Models.Requests;
using MQTTnet.Extensions.ManagedClient;

namespace Home.Devices.Zigbee.Devices {

    [Device]
    public partial class ZigbeePlugDevice : ZigbeeDevice, IOnOffDevice {

        [DeviceProperty]
        public bool On { get; protected set; }

        [DeviceCommand]
        public async Task TurnOnAsync() {
            await SendRequestAsync(new StateRequest("ON"));
            On = true;
            NotifyObservers(nameof(On), On);
        }

        [DeviceCommand]
        public async Task TurnOffAsync() {
            await SendRequestAsync(new StateRequest("OFF"));
            On = false;
            NotifyObservers(nameof(On), On);
        }

        [DeviceCommand]
        public async Task ToggleOnOffAsync() {
            if (On) {
                await TurnOffAsync();
            } else {
                await TurnOnAsync();
            }
        }

        public ZigbeePlugDevice(HomeConfigurationModel home, DeviceModel model, IManagedMqttClient mqtt, ZigbeeConfiguration configuration) : base(home, model, mqtt, configuration) {
            Type = Helpers.GetTypeString(Helpers.DeviceType.Outlet);
        }

        public override void ProcessZigbeeUpdate(DeviceUpdate update, bool isRetainedUpdate) {
            var on = update.State == "ON" && Reachable;
            if (On != on) {
                On = on;
                NotifyObservers(nameof(On), On, isRetainedUpdate);
            }
            if (LastSeen != update.LastSeen) {
                LastSeen = update.LastSeen;
                NotifyObservers(nameof(LastSeen), LastSeen, isRetainedUpdate);
            }
        }

        public override void UpdateAvailability(bool available) {
            base.UpdateAvailability(available);
            On = On && Reachable;
        }

    }

}