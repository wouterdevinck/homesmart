using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Home.Core.Interfaces {

    public interface IDevice {

        event EventHandler<DeviceUpdateEventArgs> DeviceUpdate;

        string Name { get; }
        string Manufacturer { get; }
        string DeviceId { get; }
        string FriendlyId { get; }
        string Type { get; }

        bool HasId(string id);

        Task InvokeCommand(string command, Dictionary<string, object> args = null);

    }

}

