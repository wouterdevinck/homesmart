using System.Collections.Generic;
using System.Linq;
using Home.Core.Interfaces;

namespace Home.Core {

    public abstract class AbstractAutomation : IAutomation {

        private readonly List<string> _deviceIds;
        private IDeviceProvider _provider;

        protected Dictionary<string, IDevice> Devices;

        public string Description { get; set; }
        public bool Started { get; private set; }
        public abstract string Type { get; }

        public AbstractAutomation(List<string> deviceIds) {
            _deviceIds = deviceIds;
            Devices = new Dictionary<string, IDevice>();
            Started = false;
        }

        public void Install(IDeviceProvider provider) {
            _provider = provider;
            var devices = _provider.GetDevices();
            foreach (var id in _deviceIds) {
                var device = devices.SingleOrDefault(x => x.HasId(id));
                if (device != null) {
                    Devices.Add(id, device);
                }
            }
            if (Devices.Count < _deviceIds.Count) {
                _provider.DeviceDiscovered += OnDeviceDiscovered;
            } else {
                Start();
                Started = true;
            }
        }

        private void OnDeviceDiscovered(object _, IDevice device) {
            var id = _deviceIds.SingleOrDefault(x => device.HasId(x));
            if (!string.IsNullOrEmpty(id)) {
                Devices.Add(id, device);
                if (Devices.Count == _deviceIds.Count) {
                    _provider.DeviceDiscovered -= OnDeviceDiscovered;
                    Start();
                    Started = true;
                }
            }
        }

        protected abstract void Start();

    }

}