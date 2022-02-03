using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Home.Core.Interfaces;

namespace Home.Core {

    public abstract class AbstractDeviceProvider : IDeviceProvider {

        public event EventHandler<IDevice> DeviceDiscovered;

        private readonly List<IAutomation> _automations = new();

        public abstract IEnumerable<IDevice> GetDevices();
        public abstract Task ConnectAsync();
        public abstract Task DisconnectAsync();

        protected void NotifyObservers(IEnumerable<IDevice> devices) {
            foreach (var device in devices) {
                DeviceDiscovered?.Invoke(this, device);
            }
        }

        protected void NotifyObservers(IDevice device) {
            DeviceDiscovered?.Invoke(this, device);
        }

        public void InstallAutomation(string description, IAutomation automation) {
            automation.Description = description;
            _automations.Add(automation);
            automation.Install(this);
        }

        public IEnumerable<IAutomation> GetAutomations() {
            return _automations;
        }

    }

}