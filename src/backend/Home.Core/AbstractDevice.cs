using System;
using Home.Core.Interfaces;

namespace Home.Core {

    public abstract class AbstractDevice : IDevice {

        public string Name { get; protected init; }
        public string Manufacturer { get; protected init; }
        public string DeviceId { get; protected init; }
        public string FriendlyId { get; protected init; } 
        public string Type { get; protected init; } 
  
        public event EventHandler<DeviceUpdateEventArgs> DeviceUpdate;

        protected void NotifyObservers(string property, object value) {
            DeviceUpdate?.Invoke(this, new DeviceUpdateEventArgs(property, value));
        }

        public bool HasId(string id) {
            return FriendlyId.Equals(id) || DeviceId.Equals(id);
        }

    }

}
