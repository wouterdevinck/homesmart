using System.Collections.Generic;
using Home.Core;
using Home.Core.Attributes;
using Home.Core.Configuration.Models;
using Home.Core.Devices;
using Home.Devices.Zigbee.Models;
using MQTTnet.Extensions.ManagedClient;

namespace Home.Devices.Zigbee.Devices {

    [Device]
    public partial class ZigbeeSwitchDevice : ZigbeeDevice, IBatteryDevice {

        [DeviceProperty]
        public double Battery { get; private set; }

        [DeviceProperty]
        public string Action { get; private set; }

        [DeviceProperty]
        public byte Brightness { get; private set; }
        
        public ZigbeeSwitchDevice(List<DeviceConfigurationModel> models, DeviceModel model, IManagedMqttClient mqtt, ZigbeeConfiguration configuration) : base(models, model, mqtt, configuration) {
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
            }
            if (Brightness != update.Brightness) {
                Brightness = update.Brightness;
                NotifyObservers(nameof(Brightness), Brightness);
            }
            if (LastSeen != update.LastSeen) {
                LastSeen = update.LastSeen;
                NotifyObservers(nameof(LastSeen), LastSeen);
            }
        }

    }

}