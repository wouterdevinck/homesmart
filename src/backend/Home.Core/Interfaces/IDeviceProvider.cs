using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Home.Core.Interfaces {

    public interface IDeviceProvider {

        event EventHandler<IDevice> DeviceDiscovered;

        Task ConnectAsync();
        Task DisconnectAsync();
        
        IEnumerable<IDevice> GetDevices();

    }

}