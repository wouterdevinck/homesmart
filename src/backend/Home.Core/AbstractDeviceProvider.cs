using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Home.Core.Interfaces;

namespace Home.Core {

    public abstract class AbstractDeviceProvider : IDeviceProvider {

        public event EventHandler<IDevice> DeviceDiscovered;

        public abstract IEnumerable<IDevice> GetDevices();
        public abstract Task ConnectAsync();
        public abstract Task DisconnectAsync();
        
        protected void NotifyObservers(IEnumerable<IDevice> devices) {
            foreach (var device in devices) {
                DeviceDiscovered?.Invoke(this, device);
            }
        }

    }

}