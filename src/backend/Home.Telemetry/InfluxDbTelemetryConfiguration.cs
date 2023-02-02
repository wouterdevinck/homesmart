using System.Collections.Generic;
using Home.Core.Configuration;

namespace Home.Telemetry {

    public class InfluxDbTelemetryConfiguration : ITelemetryConfiguration {

        public string Url { get; set; }
        public string Token { get; set; }
        public List<DataPoint> Points { get; set; }

    }

    public class DataPoint {

        public string DeviceId { get; set; }
        public List<string> Parameters { get; set; }

    }

}
