using System;
using Home.Core.Interfaces;
using InfluxDB.Client.Core.Flux.Domain;

namespace Home.Telemetry {

    public class InfluxDbDataPoint : IDataPoint {

        public DateTime Time { get; }
        public double Value { get; }

        public InfluxDbDataPoint(FluxRecord record) {

            record.Values.TryGetValue("_time", out var time);
            record.Values.TryGetValue("_value", out var value);

            if (time == null || !DateTime.TryParse(time.ToString(), out var dt)) {
                throw new ArgumentException("Invalid date");
            }

            Time = dt.ToLocalTime();

            if (value == null) {
                Value = 0;
            } else {
                Value = value switch {
                    double d => d,
                    int i => i,
                    long l => l,
                    _ => 0
                };
            }

        }

    }

}
 