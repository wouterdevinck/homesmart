using System.Collections.Generic;
using System.Threading.Tasks;
using Home.Core.Models;

namespace Home.Core.Interfaces {

    public interface ITelemetry : IDeviceConsumer {

        Task<IEnumerable<IDataPoint>> GetAllData(string device, string point, TimeRange range);
        Task<IEnumerable<IDataPoint>> GetWindowDifference(string device, string point, TimeRange range, RelativeTime window);
        Task<IEnumerable<IDataPoint>> GetWindowMean(string device, string point, TimeRange range, RelativeTime window);

        Task<IEnumerable<DataPointsMetadata>> GetMetadata();
        Task ExportCsv(string path);

    }

}
