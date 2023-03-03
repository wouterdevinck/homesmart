using System;
using Home.Core.Interfaces;
using InfluxDB.Client.Core.Flux.Domain;

namespace Home.Telemetry {

    public class InfluxDbDataPoint : IDataPoint {

        public DateTime Time { get; private set; }
        public double Value { get; private set; }

        public InfluxDbDataPoint(FluxRecord record) {
            Time =  DateTime.Parse(record.Values["_time"].ToString());
            Value = record.Values["_value"] as double? ?? 0;
        }

    }

}
 