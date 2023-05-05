using System;
using System.Linq;
using Home.Core.Attributes;
using Home.Core.Configuration.Models;
using Home.Core.Interfaces;

namespace Home.Core {

    [Device]
    public abstract partial class AbstractDevice : IDevice {

        [DeviceProperty]
        public string Name { get; protected set; }

        [DeviceProperty]
        public string Manufacturer { get; protected set; }

        [DeviceProperty]
        public string DeviceId { get; protected set; }

        [DeviceProperty]
        public string FriendlyId { get; protected set; }

        [DeviceProperty]
        public string RoomId { get; protected set; }

        [DeviceProperty]
        public string Type { get; protected set; }

        [DeviceProperty]
        public string Model { get; protected set; }

        [DeviceProperty]
        public string Version { get; protected set; }

        [DeviceProperty]
        public bool Reachable { get; protected set; }

        public event EventHandler<DeviceUpdateEventArgs> DeviceUpdate;

        public AbstractDevice(HomeConfigurationModel home, string id) {
            DeviceId = id;
            var dm = home.Devices?.SingleOrDefault(x => x.DeviceId == id);
            FriendlyId = dm?.FriendlyId ?? id;
            RoomId = dm?.RoomId;
        }

        protected void NotifyObservers(string property, object value, DateTime timestamp) {
            DeviceUpdate?.Invoke(this, new DeviceUpdateEventArgs(property, value, timestamp));
        }

        protected void NotifyObservers(string property, object value) {
            DeviceUpdate?.Invoke(this, new DeviceUpdateEventArgs(property, value, DateTime.UtcNow));
        }

        public bool HasId(string id) {
            return (!string.IsNullOrEmpty(FriendlyId) && FriendlyId.Equals(id)) || DeviceId.Equals(id);
        }
        
    }

}
