using System.Collections.Generic;
using Home.Core.Configuration.Interfaces;

namespace Home.Telemetry {

    public class InfluxDbTelemetryConfiguration : IDeviceConsumerConfiguration {

        public string Url { get; set; }
        public string Token { get; set; }
        public string Bucket { get; set; }
        public string Organization { get; set; }
        public List<DataPoint> Points { get; set; }

    }

    public class DataPoint {

        public string DeviceId { get; set; }
        public List<string> Properties { get; set; }

    }

}
