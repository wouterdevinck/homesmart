using System.Text;
using Home.Core.Models;

namespace Home.Telemetry {

    internal static class FluxQuery {

        public static string AllData(string bucket, string device, string point, TimeRange range) {
            return
                "import \"strings\"\n" +
                "import \"timezone\"\n" +
                "option location = timezone.location(name: \"Europe/Brussels\")\n" +
                $"from(bucket:\"{bucket}\")\n" +
                $" |> range({TimeRangeToFlux(range)})\n" +
                $" |> filter(fn: (r) => strings.toLower(v: r[\"_measurement\"]) == \"{point.ToLower()}\")\n" +
                " |> filter(fn: (r) => r[\"_field\"] == \"value\")\n" +
                $" |> filter(fn: (r) => strings.toLower(v: r[\"device\"]) == \"{device.ToLower()}\")\n";
        }

        public static string AllData(string bucket) {
            return
                $"from(bucket:\"{bucket}\")\n" +
                " |> range(start: -10y)\n" +
                " |> filter(fn: (r) => r[\"_field\"] == \"value\")\n";
        }

        public static string DiffWindow(string bucket, string device, string point, TimeRange range, RelativeTime window) {
            return
                AllData(bucket, device, point, range) +
                $" |> aggregateWindow(every: duration(v: \"{window}\"), fn: last, createEmpty: false)\n" +
                " |> difference(nonNegative: true)\n";
        }

        public static string MeanWindow(string bucket, string device, string point, TimeRange range, RelativeTime window) {
            return
                AllData(bucket, device, point, range) +
                $" |> aggregateWindow(every: duration(v: \"{window}\"), fn: mean, createEmpty: false)\n" +
                " |> yield(name: \"mean\")\n";
        }

        public static string Measurements(string bucket) {
            return
                "import \"influxdata/influxdb/schema\"\n" +
                $"schema.measurements(bucket: \"{bucket}\")";
        }

        public static string Devices(string bucket, string measurement) {
            return
                "import \"influxdata/influxdb/schema\"\n" +
                $"schema.measurementTagValues(bucket: \"{bucket}\", tag: \"device\", measurement: \"{measurement}\")";
        }

        // Note: RelativeTime implements a subset of duration https://docs.influxdata.com/flux/v0.x/data-types/basic/duration/
        //    Not all unit specifiers are implemented and neither are combinations of multiple units.
        private static string TimeRangeToFlux(TimeRange range) {
            var str = new StringBuilder("start: ");
            if (range.Type == TimeRangeType.Absolute) {
                str.Append(range.AbsoluteStartEpoch);
                if (range.AbsoluteStop != null) {
                    str.Append(", stop: ");
                    str.Append(range.AbsoluteStopEpoch);
                }
            } else {
                str.Append("-");
                str.Append(range.RelativeStart);
                if (range.RelativeStop != null) {
                    str.Append(", stop: -");
                    str.Append(range.RelativeStop);
                }
            }
            return str.ToString();
        }

    }

}
