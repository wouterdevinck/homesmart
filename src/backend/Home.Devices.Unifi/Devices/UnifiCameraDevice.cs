using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Home.Core;
using Home.Core.Attributes;
using Home.Core.Configuration.Models;
using Home.Core.Devices;
using Home.Core.Relationships;
using Home.Devices.Unifi.Models;

namespace Home.Devices.Unifi.Devices {

    [Device]
    public partial class UnifiCameraDevice : UnifiDevice, ICamera {

        private readonly TimeSpan _transitionTime = TimeSpan.FromSeconds(90);

        private readonly Mutex _mutex;
        private DateTime _desiredOnTime;
        private bool _desiredOn; // TODO Use Twin.cs

        [DeviceProperty]
        public bool On {
            get => _desiredOn;
            set => throw new NotImplementedException(); // Used in auto-generated Update method
        }

        public UnifiCameraDevice(HomeConfigurationModel home, ProtectDeviceModel device, ClientModel client) : base(home, device, $"UNIFI-PROTECT-{device.Id}") {
            Type = Helpers.GetTypeString(Helpers.DeviceType.Camera);
            Ip = client?.Ip;
            UplinkMac = device.UplinkMac;
            if (client != null) UplinkPort = client.UplinkPort;
            _desiredOn = Reachable;
            _mutex = new Mutex();
        }

        [DeviceCommand]
        public Task ToggleOnOffAsync() {
            return On ? TurnOffAsync() : TurnOnAsync();
        }

        [DeviceCommand]
        public async Task TurnOnAsync() {
            if (RelatedDevices.SingleOrDefault(x => x is ParentNetworkSwitch && x.Device is IPoeNetworkSwitch) is ParentNetworkSwitch { Device: IPoeNetworkSwitch device, SwitchPort: var port }) {
                if (await device.TurnPortPowerOnAsync(port)) {
                    _mutex.WaitOne();
                    _desiredOnTime = DateTime.Now;
                    _desiredOn = true;
                    _mutex.ReleaseMutex();
                    NotifyObservers(nameof(On), On);
                }
            } else {
                throw new NotImplementedException();
            }
        }

        [DeviceCommand]
        public async Task TurnOffAsync() {
            if (RelatedDevices.SingleOrDefault(x => x is ParentNetworkSwitch && x.Device is IPoeNetworkSwitch) is ParentNetworkSwitch { Device: IPoeNetworkSwitch device, SwitchPort: var port }) {
                if (await device.TurnPortPowerOffAsync(port)) {
                    _mutex.WaitOne();
                    _desiredOnTime = DateTime.Now;
                    _desiredOn = false;
                    _mutex.ReleaseMutex();
                    NotifyObservers(nameof(On), On);
                }
            } else {
                throw new NotImplementedException();
            }
        }

        public void ProcessUpdate(ProtectDeviceModel update) {
            if (UplinkMac != update.UplinkMac) {
                UplinkMac = update.UplinkMac;
                NotifyObservers(nameof(UplinkMac), UplinkMac);
            }
            if (_desiredOn != update.Online) {
                if (_desiredOnTime < DateTime.Now - _transitionTime) {
                    _desiredOn = update.Online;
                    NotifyObservers(nameof(On), On);
                }
            }
            base.ProcessUpdate(update);
        }

        public void ProcessUpdate(ClientModel update) {
            if (Ip != update.Ip) {
                Ip = update.Ip;
                NotifyObservers(nameof(Ip), Ip);
            }
            if (UplinkPort != update.UplinkPort) {
                UplinkPort = update.UplinkPort;
                NotifyObservers(nameof(UplinkPort), UplinkPort);
            }
        }

    }

}
