using System.Collections.Generic;

namespace Home.Core.Data {

    public class DataPointsMetadata(string deviceId, List<string> points) {

        public string DeviceId { get; } = deviceId;
        public List<string> Points { get; } = points;

    }

}