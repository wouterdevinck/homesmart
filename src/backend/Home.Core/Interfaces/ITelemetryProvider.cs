using System.Collections.Generic;
using System.Threading.Tasks;
using Home.Core.Models;

namespace Home.Core.Interfaces {

    public interface ITelemetryProvider {

        Task<IEnumerable<IDataPoint>> GetDataAsync(string device, string point, TimeRange range);

    }

}
