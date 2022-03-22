using System;
using System.Collections.Generic;
using Home.Core;
using Home.Core.Configuration.Models;
using Home.Core.Devices;
using Home.Devices.Zigbee.Models;
using MQTTnet.Extensions.ManagedClient;

namespace Home.Devices.Zigbee.Devices {

    public class ZigbeeButtonDevice : ZigbeeDevice, IPushButton, IBatteryDevice {

        public event EventHandler SinglePress;

        public double Battery { get; private set; }
        public string Action { get; private set; }

        public ZigbeeButtonDevice(List<DeviceConfigurationModel> models, DeviceModel model, IManagedMqttClient mqtt, ZigbeeConfiguration configuration) : 
            base(models, model, mqtt, configuration) {
            Type = Helpers.GetTypeString(Helpers.DeviceType.Switch);
        }

        public override void ProcessZigbeeUpdate(DeviceUpdate update) {
            if (Battery != update.Battery) {
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
