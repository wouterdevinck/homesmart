using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
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

        // TODO Support arguments, look into implementing this using a generator again? See v3
        public async Task InvokeCommand(string command, Dictionary<string, object> args = null) {
            if (args != null && args.Count > 0) {
                throw new NotImplementedException();
            }
            var method = GetType().GetMethod($"{command.FirstCharToUpperCase()}Async");
            await method.InvokeAsync(this);
        }

    }

    // TEMP To support InvokeCommand using reflection above
    // https://stackoverflow.com/questions/39674988/how-to-call-a-generic-async-method-using-reflection
    public static class ExtensionMethods {
        public static async Task<object> InvokeAsync(this MethodInfo @this, object obj, params object[] parameters) {
            var task = (Task)@this.Invoke(obj, parameters);
            await task.ConfigureAwait(false);
            var resultProperty = task.GetType().GetProperty("Result");
            return resultProperty.GetValue(task);
        }
    }

}
