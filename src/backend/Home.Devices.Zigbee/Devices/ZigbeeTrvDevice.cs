using System;
using System.Threading;
using System.Threading.Tasks;
using Home.Core;
using Home.Core.Attributes;
using Home.Core.Configuration.Models;
using Home.Core.Devices;
using Home.Core.Models;
using Home.Devices.Zigbee.Models;
using Home.Devices.Zigbee.Models.Requests;
using MQTTnet.Extensions.ManagedClient;
using Timer = System.Timers.Timer;

namespace Home.Devices.Zigbee.Devices {

    [Device]
    public partial class ZigbeeTrvDevice : ZigbeeDevice, ITrv {

        private const int TimerTickMs = 1000;
        private const int AllowedTimeoutMs = 10000;

        private const double OffTemp = 5.0;
        private double _latestOnTemp = 22.0;

        private readonly Twin<bool> _on;
        private readonly Twin<double> _requestedTemperature;

        private readonly Timer _timer;

        [DeviceProperty]
        public bool On {
            get => _on.Desired;
            set => throw new NotImplementedException(); // Used in auto-generated Update method
        }

        [DeviceProperty] 
        public double Temperature { get; private set; }

        [DeviceProperty] 
        public double RequestedTemperature {
            get => _requestedTemperature.Desired;
            set => throw new NotImplementedException(); // Used in auto-generated Update method
        }

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
            _on = new Twin<bool>(TimeSpan.FromMilliseconds(AllowedTimeoutMs), (b1, b2) => b1 == b2);
            _requestedTemperature = new Twin<double>(TimeSpan.FromMilliseconds(AllowedTimeoutMs), (t1, t2) => Math.Abs(t1 - t2) < Tolerance);
            _timer = new Timer(TimerTickMs);
            _timer.Elapsed += (_, __) => TwinStateMachine(TwinEventType.Timer);
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }

        [DeviceCommand]
        public async Task SetRequestedTemperatureAsync(double t) {
            await SendRequestAsync(new TemperatureRequest(t));
            TwinStateMachine(TwinEventType.Desired, t: t);
        }

        [DeviceCommand]
        public async Task TurnOnAsync() {
            await SendRequestAsync(new ModeRequest("heat"));
            TwinStateMachine(TwinEventType.Desired, on: true);
        }

        [DeviceCommand]
        public async Task TurnOffAsync() {
            await SendRequestAsync(new ModeRequest("off"));
            TwinStateMachine(TwinEventType.Desired, on: false);
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
            var on = update.SystemMode == "heat" && Reachable; // Note: mode "auto" not used/supported
            TwinStateMachine(TwinEventType.Reported, on, update.RequestedTemperature, isRetainedUpdate);
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
            if (ValvePosition != update.ValvePosition) {
                ValvePosition = update.ValvePosition;
                NotifyObservers(nameof(ValvePosition), ValvePosition, isRetainedUpdate);
            }
            var heating = update.RunningState == "heat" && Reachable;
            if (IsHeating != heating) {
                IsHeating = heating;
                NotifyObservers(nameof(IsHeating), IsHeating, isRetainedUpdate);
            }
        }

        private void TwinStateMachine(TwinEventType e, bool? on = null, double? t = null, bool isRetainedUpdate = false) {
            switch (e) {
                case TwinEventType.Desired:
                    if (on != null) {
                        _on.Desired = on.Value;
                        NotifyObservers(nameof(On), On);
                        _requestedTemperature.Desired = on.Value ? _latestOnTemp : OffTemp;
                        NotifyObservers(nameof(RequestedTemperature), RequestedTemperature);
                    }
                    if (t != null) {
                        _requestedTemperature.Desired = t.Value;
                        NotifyObservers(nameof(RequestedTemperature), RequestedTemperature);
                    }
                    break;
                case TwinEventType.Reported:
                    if (on != null) {
                        var ret = _on.Report(on.Value);
                        if (ret == TwinReportResult.Settled || ret == TwinReportResult.Update) {
                            NotifyObservers(nameof(On), On, isRetainedUpdate);
                        }
                    }
                    if (t != null) {
                        _latestOnTemp = t.Value;
                        var ret = _requestedTemperature.Report(t.Value);
                        if (ret == TwinReportResult.Settled || ret == TwinReportResult.Update) {
                            NotifyObservers(nameof(RequestedTemperature), RequestedTemperature, isRetainedUpdate);
                        }
                    }
                    break;
                case TwinEventType.Timer:
                    if (_on.State == TwinState.Expired) {
                        _on.Revert();
                        NotifyObservers(nameof(On), On);
                    }
                    if (_requestedTemperature.State == TwinState.Expired) {
                        _requestedTemperature.Revert();
                        NotifyObservers(nameof(RequestedTemperature), RequestedTemperature);
                    }
                    break;
            }
        }

    }

}