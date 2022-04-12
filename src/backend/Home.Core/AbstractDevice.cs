using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Home.Core.Interfaces;

namespace Home.Core {

    public abstract class AbstractDevice : IDevice {

        public string Name { get; protected init; }
        public string Manufacturer { get; protected init; }
        public string DeviceId { get; protected init; }
        public string FriendlyId { get; protected init; } 
        public string Type { get; protected init; }
        public string Model { get; protected init; }
        public string Version { get; protected init; }
        public bool Reachable { get; protected set; }

        public event EventHandler<DeviceUpdateEventArgs> DeviceUpdate;

        protected void NotifyObservers(string property, object value) {
            DeviceUpdate?.Invoke(this, new DeviceUpdateEventArgs(property, value));
        }

        public bool HasId(string id) {
            return FriendlyId.Equals(id) || DeviceId.Equals(id);
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
