using Home.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Home.Core.Interfaces {

    public interface ITelemetry : IDeviceConsumer {

        Task<IEnumerable<IDataPoint>> GetDataAsync(string device, string point, TimeRange range);

    }

}
