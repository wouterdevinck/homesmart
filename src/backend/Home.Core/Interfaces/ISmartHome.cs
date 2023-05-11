using Home.Core.Configuration.Models;
using Home.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Home.Core.Interfaces {

    public interface ISmartHome : IDeviceProvider {
        
        IEnumerable<RoomConfigurationModel> GetRooms();
        IEnumerable<IDeviceProvider> GetProviders();
        IEnumerable<IAutomation> GetAutomations();
        ITelemetry GetTelemetry();
        IRemote GetRemote();
        Task<IEnumerable<IDataPoint>> GetDataAsync(string device, string point, TimeRange range);

    }

}
