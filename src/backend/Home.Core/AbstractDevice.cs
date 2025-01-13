using System;
using System.Collections.Generic;
using System.Linq;
using Home.Core.Attributes;
using Home.Core.Configuration.Models;
using Home.Core.Interfaces;
using Newtonsoft.Json;

namespace Home.Core {

    [Device]
    public abstract partial class AbstractDevice : IDevice {

        protected double Tolerance = 0.01;

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

        [DeviceProperty]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string AlternateName { get; private set; }

        [DeviceProperty]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string AlternateIcon { get; private set; }

        [DeviceProperty]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool HideWhenUnreachable { get; private set; }

        private readonly List<IRelatedDevice<IDevice>> _relatedDevices;

        [JsonIgnore]
        public IEnumerable<IRelatedDevice<IDevice>> RelatedDevices => _relatedDevices;

        public event EventHandler<DeviceUpdateEventArgs> DeviceUpdate;

        protected AbstractDevice(HomeConfigurationModel home, string id) {
            DeviceId = id;
            var dm = home.Devices?.SingleOrDefault(x => x.DeviceId == id);
            FriendlyId = dm?.FriendlyId ?? id;
            RoomId = dm?.RoomId;
            AlternateName = dm?.AlternateName;
            AlternateIcon = dm?.AlternateIcon;
            HideWhenUnreachable = dm?.HideWhenUnreachable ?? false;
            _relatedDevices = [];
        }

        protected void NotifyObservers(string property, object value, DateTime timestamp, bool retained) {
            DeviceUpdate?.Invoke(this, new DeviceUpdateEventArgs(property, value, timestamp, retained));
        }

        protected void NotifyObservers(string property, object value, DateTime timestamp) {
            DeviceUpdate?.Invoke(this, new DeviceUpdateEventArgs(property, value, timestamp, false));
        }

        protected void NotifyObservers(string property, object value, bool retained) {
            DeviceUpdate?.Invoke(this, new DeviceUpdateEventArgs(property, value, DateTime.UtcNow, retained));
        }

        protected void NotifyObservers(string property, object value) {
            DeviceUpdate?.Invoke(this, new DeviceUpdateEventArgs(property, value, DateTime.UtcNow, false));
        }

        public void AddRelatedDevice(IRelatedDevice<IDevice> related) {
            _relatedDevices.Add(related);
        }

        public bool HasId(string id) {
            return (!string.IsNullOrEmpty(FriendlyId) && FriendlyId.Equals(id)) || DeviceId.Equals(id);
        }

        public PropertyInfo GetPropertyInfo(string point) {
            var prop = this.GetType().GetProperties().SingleOrDefault(x => x.Name.Equals(point, StringComparison.InvariantCultureIgnoreCase));
            if (prop != null) {
                var attr = prop.GetCustomAttributes(false).SingleOrDefault(x => x.GetType().Name.Contains("DevicePropertyAttribute"));
                if (attr != null) {
                    return new PropertyInfo((attr as DevicePropertyAttribute)?.Unit);
                }
            }
            return null;
        }

    }

}
