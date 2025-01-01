using System;
using Home.Core;
using Home.Core.Attributes;
using Home.Core.Configuration.Models;
using Home.Core.Devices;
using Home.Devices.Zigbee.Models;
using MQTTnet.Extensions.ManagedClient;

namespace Home.Devices.Zigbee.Devices {

    [Device]
    public partial class ZigbeeDimmerDevice : ZigbeeDevice, IDimmer, IBatteryDevice {

        public event EventHandler On;
        public event EventHandler Up;
        public event EventHandler Down;
        public event EventHandler Off;

        // TODO Support pressing and holding buttons with release events and action_duration property

        [DeviceProperty<double>(Unit = "%", Min = 0, Max = 100)]
        public double Battery { get; private set; }

        [DeviceProperty]
        public string Action { get; private set; }

        public ZigbeeDimmerDevice(HomeConfigurationModel home, DeviceModel model, IManagedMqttClient mqtt, ZigbeeConfiguration configuration) : 
            base(home, model, mqtt, configuration) {
            Type = Helpers.GetTypeString(Helpers.DeviceType.Switch);
        }

        public override void ProcessZigbeeUpdate(DeviceUpdate update, bool isRetainedUpdate) {
            if (Math.Abs(Battery - update.Battery) >= Tolerance) {
                Battery = update.Battery;
                NotifyObservers(nameof(Battery), Battery, isRetainedUpdate);
            }
            if (!string.IsNullOrEmpty(update.Action)) {
                Action = update.Action;
                NotifyObservers(nameof(Action), Action, isRetainedUpdate);
                if (!isRetainedUpdate) {
                    if (update.Action is "on_press_release" or "on-press" or "on-hold-release") {
                        On?.Invoke(this, EventArgs.Empty);
                    } else if (update.Action is "up_press_release" or "up-press" or "up-hold-release") {
                        Up?.Invoke(this, EventArgs.Empty);
                    } else if (update.Action is "down_press_release" or "down-press" or "down-hold-release") {
                        Down?.Invoke(this, EventArgs.Empty);
                    } else if (update.Action is "off_press_release" or "off-press" or "off-hold-release") {
                        Off?.Invoke(this, EventArgs.Empty);
                    }
                }
            }
            if (LastSeen != update.LastSeen) {
                LastSeen = update.LastSeen;
                NotifyObservers(nameof(LastSeen), LastSeen, isRetainedUpdate);
            }
        }

    }

}
