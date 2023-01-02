using System.Collections.Generic;
using System.Threading.Tasks;
using Home.Core;
using Home.Core.Configuration.Models;
using Home.Core.Devices;
using Home.Devices.Zigbee.Models;
using MQTTnet.Extensions.ManagedClient;

namespace Home.Devices.Zigbee.Devices {

    public class ZigbeePlugDevice : ZigbeeDevice, IOnOffDevice {

        public bool On { get; protected set; }

        public async Task TurnOnAsync() {
            await Mqtt.EnqueueAsync($"{_configuration.BaseTopic}/{Name}/set", "{\"state\":\"ON\"}");
            On = true;
            NotifyObservers("on", On);
            // TODO Use model for JSON payload
            // TODO Check if successful
            // TODO Return result
        }

        public async Task TurnOffAsync() {
            await Mqtt.EnqueueAsync($"{_configuration.BaseTopic}/{Name}/set", "{\"state\":\"OFF\"}");
            On = false;
            NotifyObservers("on", On);
            // TODO Use model for JSON payload
            // TODO Check if successful
            // TODO Return result
        }

        public async Task ToggleOnOffAsync() {
            if (On) {
                await TurnOffAsync();
            } else {
                await TurnOnAsync();
            }
        }

        public ZigbeePlugDevice(List<DeviceConfigurationModel> models, DeviceModel model, IManagedMqttClient mqtt, ZigbeeConfiguration configuration) : base(models, model, mqtt, configuration) {
            Type = Helpers.GetTypeString(Helpers.DeviceType.Outlet);
        }

        public override void ProcessZigbeeUpdate(DeviceUpdate update) {
            var on = update.State == "ON" && Reachable;
            if (On != on) {
                On = on;
                NotifyObservers(nameof(On), On);
            }
            if (LastSeen != update.LastSeen) {
                LastSeen = update.LastSeen;
                NotifyObservers(nameof(LastSeen), LastSeen);
            }
        }

        public override void UpdateAvailability(bool available) {
            base.UpdateAvailability(available);
            On = On && Reachable;
        }

    }

}