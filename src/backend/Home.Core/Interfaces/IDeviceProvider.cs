using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Home.Core.Interfaces {

    public interface IDeviceProvider {

        event EventHandler<IDevice> DeviceDiscovered;

        Task ConnectAsync();
        Task DisconnectAsync();

        void InstallAutomation(string description, IAutomation automation);
        void InstallTelemetry(ITelemetry automation);

        IEnumerable<IDevice> GetDevices();
        IEnumerable<IAutomation> GetAutomations();


    }

}