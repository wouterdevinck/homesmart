using System;
using Home.Core;
using Home.Core.Attributes;
using Home.Core.Configuration.Models;
using Home.Core.Devices;
using Home.Devices.Zigbee.Models;
using MQTTnet.Extensions.ManagedClient;

namespace Home.Devices.Zigbee.Devices {

    [Device]
    public partial class ZigbeeButtonDevice : ZigbeeDevice, IPushButton, IBatteryDevice {

        public event EventHandler SinglePress;

        [DeviceProperty]
        public double Battery { get; private set; }

        [DeviceProperty]
        public string Action { get; private set; }

        public ZigbeeButtonDevice(HomeConfigurationModel home, DeviceModel model, IManagedMqttClient mqtt, ZigbeeConfiguration configuration) : 
            base(home, model, mqtt, configuration) {
            Type = Helpers.GetTypeString(Helpers.DeviceType.Switch);
        }

        public override void ProcessZigbeeUpdate(DeviceUpdate update, bool isRetainedUpdate) {
            if (Math.Abs(Battery - update.Battery) >= Tolerance) {
                Battery = update.Battery;
                NotifyObservers(nameof(Battery), Battery);
            }
            if (!string.IsNullOrEmpty(update.Action)) {
                Action = update.Action;
                NotifyObservers(nameof(Action), Action);
                if ((Model == "WXKG11LM" && update.Action == "single") ||
                    (Model == "E1812" && update.Action == "on")) {
                    SinglePress?.Invoke(this, null);
                }
            }
            if (LastSeen != update.LastSeen) {
                LastSeen = update.LastSeen;
                NotifyObservers(nameof(LastSeen), LastSeen);
            }
        }

    }

}
