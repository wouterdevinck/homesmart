using System;

namespace Home.Core.Interfaces {

    public interface IDevice {

        event EventHandler<DeviceUpdateEventArgs> DeviceUpdate;

        string Name { get; }
        string Manufacturer { get; }
        string DeviceId { get; }
        string FriendlyId { get; }
        string Type { get; }

        bool HasId(string id);

    }

}

