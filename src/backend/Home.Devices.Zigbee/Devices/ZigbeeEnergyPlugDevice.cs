using Home.Core.Attributes;
using Home.Core.Configuration.Models;
using Home.Core.Devices;
using Home.Devices.Zigbee.Models;
using MQTTnet.Extensions.ManagedClient;

namespace Home.Devices.Zigbee.Devices {

    [Device]
    public partial class ZigbeeEnergyPlugDevice : ZigbeePlugDevice, ILockable, IEnergySensor {

        [DeviceProperty]
        public bool Locked { get; private set; }

        [DeviceProperty]
        public double Current { get; private set; }

        [DeviceProperty]
        public double Power { get; private set; }

        [DeviceProperty]
        public double Voltage { get; private set; }

        [DeviceProperty]
        public double Energy { get; private set; }

        public ZigbeeEnergyPlugDevice(HomeConfigurationModel home, DeviceModel model, IManagedMqttClient mqtt, ZigbeeConfiguration configuration) : base(home, model, mqtt, configuration) {}

        public override void ProcessZigbeeUpdate(DeviceUpdate update, bool isRetainedUpdate) {
            base.ProcessZigbeeUpdate(update, isRetainedUpdate);
            var locked = update.ChildLock == "LOCK";
            if (Locked != locked) {
                Locked = locked;
                NotifyObservers(nameof(Locked), Locked);
            }
            if (!isRetainedUpdate) { // TODO Use this to update the values, without writing them to telemetry
                if (Current != update.Current) {
                    Current = update.Current;
                    NotifyObservers(nameof(Current), Current);
                }
                if (Power != update.Power) {
                    Power = update.Power;
                    NotifyObservers(nameof(Power), Power);
                }
                if (Voltage != update.Voltage) {
                    Voltage = update.Voltage;
                    NotifyObservers(nameof(Voltage), Voltage);
                }
                if (Energy != update.Energy) {
                    Energy = update.Energy;
                    NotifyObservers(nameof(Energy), Energy);
                }
            }
        }

    }

}