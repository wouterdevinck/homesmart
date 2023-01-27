using System.Collections.Generic;
using Home.Core;
using Home.Core.Attributes;
using Home.Core.Configuration.Models;
using Home.Core.Devices;
using Home.Devices.Zigbee.Models;
using MQTTnet.Extensions.ManagedClient;

namespace Home.Devices.Zigbee.Devices {

    [Device]
    public partial class ZigbeeLeakDevice : ZigbeeDevice, IBatteryDevice, IWaterLeakSensor {

        [DeviceProperty]
        public double Battery { get; private set; }
        
        [DeviceProperty]
        public bool Leak { get; private set; }

        public ZigbeeLeakDevice(List<DeviceConfigurationModel> models, DeviceModel model, IManagedMqttClient mqtt, ZigbeeConfiguration configuration) : base(models, model, mqtt, configuration) {
            Type = Helpers.GetTypeString(Helpers.DeviceType.Leak);
        }

        public override void ProcessZigbeeUpdate(DeviceUpdate update) {
            if (Battery != update.Battery) {
                Battery = update.Battery;
                NotifyObservers(nameof(Battery), Battery);
            }
            if (Leak != update.WaterLeak) {
                Leak = update.WaterLeak;
                NotifyObservers(nameof(Leak), Leak);
            }
            if (LastSeen != update.LastSeen) {
                LastSeen = update.LastSeen;
                NotifyObservers(nameof(LastSeen), LastSeen);
            }
        }

    }

}
