using System;
using System.Threading.Tasks;
using Home.Core;
using Home.Core.Attributes;
using Home.Core.Configuration.Models;
using Home.Core.Devices;
using Home.Devices.Zigbee.Models;
using Home.Devices.Zigbee.Models.Requests;
using MQTTnet.Extensions.ManagedClient;

namespace Home.Devices.Zigbee.Devices {

    [Device]
    public partial class ZigbeeTrvDevice : ZigbeeDevice, ITrv {
        
        [DeviceProperty] 
        public bool On { get; private set; }

        [DeviceProperty] 
        public double Temperature { get; private set; }

        [DeviceProperty]
        public double RequestedTemperature { get; private set; }

        [DeviceProperty] 
        public double Battery { get; private set; }

        [DeviceProperty]
        public bool Locked { get; private set; }

        [DeviceProperty]
        public int ValvePosition { get; private set; }

        [DeviceProperty]
        public bool IsHeating { get; private set; }

        public ZigbeeTrvDevice(HomeConfigurationModel home, DeviceModel model, IManagedMqttClient mqtt, ZigbeeConfiguration configuration) : base(home, model, mqtt, configuration) {
            Type = Helpers.GetTypeString(Helpers.DeviceType.Trv);
        }

        [DeviceCommand]
        public async Task SetRequestedTemperatureAsync(double t) {
            await SendRequestAsync(new TemperatureRequest(t));
            RequestedTemperature = t;
            NotifyObservers(nameof(RequestedTemperature), RequestedTemperature);
        }

        [DeviceCommand]
        public async Task TurnOnAsync() {
            await SendRequestAsync(new ModeRequest("manual"));
            On = true;
            NotifyObservers(nameof(On), On);
        }

        [DeviceCommand]
        public async Task TurnOffAsync() {
            await SendRequestAsync(new ModeRequest("pause"));
            On = false;
            NotifyObservers(nameof(On), On);
        }

        [DeviceCommand]
        public async Task ToggleOnOffAsync() {
            if (On) {
                await TurnOffAsync();
            } else {
                await TurnOnAsync();
            }
        }

        public override void ProcessZigbeeUpdate(DeviceUpdate update, bool isRetainedUpdate) {
            if (Math.Abs(Battery - update.Battery) >= Tolerance) {
                Battery = update.Battery;
                NotifyObservers(nameof(Battery), Battery, isRetainedUpdate);
            }
            if (LastSeen != update.LastSeen) {
                LastSeen = update.LastSeen;
                NotifyObservers(nameof(LastSeen), LastSeen, isRetainedUpdate);
            }
            var locked = update.ChildLock == "LOCK";
            if (Locked != locked) {
                Locked = locked;
                NotifyObservers(nameof(Locked), Locked, isRetainedUpdate);
            }
            if (Math.Abs(Temperature - update.LocalTemperature) >= Tolerance) {
                Temperature = update.LocalTemperature;
                NotifyObservers(nameof(Temperature), Temperature, isRetainedUpdate);
            }
            if (Math.Abs(RequestedTemperature - update.RequestedTemperature) >= Tolerance) {
                RequestedTemperature = update.RequestedTemperature;
                NotifyObservers(nameof(RequestedTemperature), RequestedTemperature, isRetainedUpdate);
            }
            if (ValvePosition != update.ValvePosition) {
                ValvePosition = update.ValvePosition;
                NotifyObservers(nameof(ValvePosition), ValvePosition, isRetainedUpdate);
            }
            var on = update.OperatingMode == "manual" && Reachable;
            if (On != on) {
                On = on;
                NotifyObservers(nameof(On), On, isRetainedUpdate);
            }
            var heating = update.RunningState == "heat" && Reachable;
            if (IsHeating != heating) {
                IsHeating = heating;
                NotifyObservers(nameof(IsHeating), IsHeating, isRetainedUpdate);
            }
        }

    }

}