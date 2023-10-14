using System;
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

        public ZigbeeTemperatureDevice(HomeConfigurationModel home, DeviceModel model, IManagedMqttClient mqtt, ZigbeeConfiguration configuration) : base(home, model, mqtt, configuration) {
            Type = Helpers.GetTypeString(Helpers.DeviceType.Temperature);
        }

        public override void ProcessZigbeeUpdate(DeviceUpdate update, bool isRetainedUpdate) {
            if (Math.Abs(Battery - update.Battery) >= Tolerance) {
                Battery = update.Battery;
                NotifyObservers(nameof(Battery), Battery, isRetainedUpdate);
            }
            if (Math.Abs(Humidity - update.Humidity) >= Tolerance) {
                Humidity = update.Humidity;
                NotifyObservers(nameof(Humidity), Humidity, isRetainedUpdate);
            }
            if (Math.Abs(Pressure - update.Pressure) >= Tolerance) {
                Pressure = update.Pressure;
                NotifyObservers(nameof(Pressure), Pressure, isRetainedUpdate);
            }
            if (Math.Abs(Temperature - update.Temperature) >= Tolerance) {
                Temperature = update.Temperature;
                NotifyObservers(nameof(Temperature), Temperature, isRetainedUpdate);
            }
            if (LastSeen != update.LastSeen) {
                LastSeen = update.LastSeen;
                NotifyObservers(nameof(LastSeen), LastSeen, isRetainedUpdate);
            }
        }

    }

}
