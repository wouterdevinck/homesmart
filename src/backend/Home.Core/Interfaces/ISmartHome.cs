using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Home.Core.Configuration.Models;

namespace Home.Core.Interfaces {

    public interface ISmartHome : IDeviceProvider {
        
        IEnumerable<RoomConfigurationModel> GetRooms();
        IEnumerable<IDeviceProvider> GetProviders();
        IEnumerable<IAutomation> GetAutomations();
        ITelemetry GetTelemetry();
        IRemote GetRemote();

        Task<IEnumerable<IDataPoint>> GetData(string id, string point, string since,string toAgo, DateTime? from, DateTime? to, string meanWindow, string diffWindow);

    }

}
