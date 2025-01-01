using System.Collections.Generic;
using Home.Core.Configuration.Interfaces;

namespace Home.Telemetry {

    public class InfluxDbTelemetryConfiguration : ITelemetryConfiguration {

        public string Url { get; set; }
        public string Token { get; set; }
        public string Bucket { get; set; }
        public string Organization { get; set; }
        public string Timezone { get; set; }
        public List<DataPoint> Data { get; set; }

    }

    public class DataPoint {

        public List<string> DeviceIds { get; set; }
        public List<string> Properties { get; set; }

    }

}
