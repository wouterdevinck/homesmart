using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Home.Core.Interfaces {

    public interface IDeviceProvider {

        event EventHandler<IDevice> DeviceDiscovered;

        Task ConnectAsync();
        Task DisconnectAsync();

        void InstallDeviceConsumer(string description, IDeviceConsumer automation);

        IEnumerable<IDevice> GetDevices();
        IEnumerable<IDeviceConsumer> GetDeviceConsumers();


    }

}