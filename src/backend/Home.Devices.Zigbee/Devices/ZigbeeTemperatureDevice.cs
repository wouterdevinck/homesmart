using System.Collections.Generic;
using Home.Core;
using Home.Core.Attributes;
using Home.Core.Configuration.Models;
using Home.Core.Devices;
using Home.Devices.Zigbee.Models;
using MQTTnet.Extensions.ManagedClient;

namespace Home.Devices.Zigbee.Devices {

    [Device]
    public partial class ZigbeeTemperatureDevice : ZigbeeDevice, IBatteryDevice, ITemperatureSensor, IHumiditySensor, IPressureSensor {

        [DeviceProperty]
        public double Battery { get; private set; }

        [DeviceProperty]
        public double Humidity { get; private set; }

        [DeviceProperty]
        public double Pressure { get; private set; }

        [DeviceProperty]
        public double Temperature { get; private set; }

        public ZigbeeTemperatureDevice(List<DeviceConfigurationModel> models, DeviceModel model, IManagedMqttClient mqtt, ZigbeeConfiguration configuration) : base(models, model, mqtt, configuration) {
            Type = Helpers.GetTypeString(Helpers.DeviceType.Temperature);
        }

        public override void ProcessZigbeeUpdate(DeviceUpdate update) {
            if (Battery != update.Battery) {
                Battery = update.Battery;
                NotifyObservers(nameof(Battery), Battery);
            }
            if (Humidity != update.Humidity) {
                Humidity = update.Humidity;
                NotifyObservers(nameof(Humidity), Humidity);
            }
            if (Pressure != update.Pressure) {
                Pressure = update.Pressure;
                NotifyObservers(nameof(Pressure), Pressure);
            }
            if (Temperature != update.Temperature) {
                Temperature = update.Temperature;
                NotifyObservers(nameof(Temperature), Temperature);
            }
            if (LastSeen != update.LastSeen) {
                LastSeen = update.LastSeen;
                NotifyObservers(nameof(LastSeen), LastSeen);
            }
        }

    }

}
