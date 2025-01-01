using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Home.Core.Configuration.Models;
using Home.Core.Models;

namespace Home.Core.Interfaces {

    public interface ISmartHome : IDeviceProvider {
        
        IEnumerable<RoomConfigurationModel> GetRooms();
        IEnumerable<IDeviceProvider> GetProviders();
        IEnumerable<IAutomation> GetAutomations();
        ITelemetry GetTelemetry();
        IRemote GetRemote();

        Task<DataSet> GetData(string deviceId, string point, string since,string toAgo, DateTime? from, DateTime? to, string meanWindow, string diffWindow);

    }

}
