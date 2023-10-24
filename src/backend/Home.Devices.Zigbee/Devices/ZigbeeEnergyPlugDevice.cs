using System;
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

        //[DeviceProperty]
        public bool Active => Current > 0;

        public ZigbeeEnergyPlugDevice(HomeConfigurationModel home, DeviceModel model, IManagedMqttClient mqtt, ZigbeeConfiguration configuration) : base(home, model, mqtt, configuration) {}

        public override void ProcessZigbeeUpdate(DeviceUpdate update, bool isRetainedUpdate) {
            base.ProcessZigbeeUpdate(update, isRetainedUpdate);
            var locked = update.ChildLock == "LOCK";
            if (Locked != locked) {
                Locked = locked;
                NotifyObservers(nameof(Locked), Locked, isRetainedUpdate);
            }
            if (Math.Abs(Current - update.Current) >= Tolerance || update.Current == 0 && Current > 0) {
                var activeBefore = Active;
                Current = update.Current;
                NotifyObservers(nameof(Current), Current, isRetainedUpdate);
                if (Active != activeBefore) {
                    NotifyObservers(nameof(Active), Active, isRetainedUpdate);
                }
            }
            if (Math.Abs(Power - update.Power) >= Tolerance) {
                Power = update.Power;
                NotifyObservers(nameof(Power), Power, isRetainedUpdate);
            }
            if (Math.Abs(Voltage - update.Voltage) >= Tolerance) {
                Voltage = update.Voltage;
                NotifyObservers(nameof(Voltage), Voltage, isRetainedUpdate);
            }
            if (Math.Abs(Energy - update.Energy) >= Tolerance) {
                Energy = update.Energy;
                NotifyObservers(nameof(Energy), Energy, isRetainedUpdate);
            }
        }

    }

}