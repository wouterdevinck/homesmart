using System.Text;
using Home.Core.Extensions;
using Home.Core.Models;

namespace Home.Telemetry {

    internal static class FluxQuery {

        private static string FluxCommonOptions() {
            return
                "import \"strings\"\n" +
                "import \"timezone\"\n" +
                "option location = timezone.location(name: \"Europe/Brussels\")\n"; // TODO Pass timezone as parameter?
        }

        private static string FluxCommonBucket(string bucket) {
            return $"from(bucket:\"{bucket}\")\n";
        }

        private static string FluxCommonFilters(string device, string point) {
            return
                $" |> filter(fn: (r) => strings.toLower(v: r[\"_measurement\"]) == \"{point.ToLower()}\")\n" +
                " |> filter(fn: (r) => r[\"_field\"] == \"value\")\n" +
                $" |> filter(fn: (r) => strings.toLower(v: r[\"device\"]) == \"{device.ToLower()}\")\n";
        }

        private static string FluxCommonDrop() {
            return " |> drop(columns: [\"_start\", \"_stop\", \"device\", \"_measurement\", \"_field\"])";
        }

        public static string AllData(string bucket, string device, string point, TimeRange range) {
            return
                FluxCommonOptions() +
                FluxCommonBucket(bucket) +
                $" |> range({TimeRangeToFlux(range)})\n" +
                FluxCommonFilters(device, point) +
                FluxCommonDrop();
        }

        public static string AllData(string bucket) {
            return
                FluxCommonBucket(bucket) +
                " |> range(start: 0)\n" +
                " |> filter(fn: (r) => r[\"_field\"] == \"value\")\n" +
                FluxCommonDrop();
        }

        public static string MeanWindow(string bucket, string device, string point, TimeRange range, RelativeTime window) {
            return
                FluxCommonOptions() +
                FluxCommonBucket(bucket) +
                $" |> range({TimeRangeToFlux(range)})\n" +
                FluxCommonFilters(device, point) +
                $" |> aggregateWindow(every: duration(v: \"{window}\"), fn: mean, createEmpty: false)\n" +
                " |> yield(name: \"mean\")\n" +
                FluxCommonDrop();
        }

        public static string DiffWindow(string bucket, string device, string point, TimeRange range, RelativeTime window) {
            return
                FluxCommonOptions() + "\n" +
                "t1 = " + FluxCommonBucket(bucket) +
                $" |> range({TimeRangeToFlux(range.BeginningOfTimeToStartOfTimeRange())})\n" +
                " |> last()\n" +
                FluxCommonFilters(device, point) + "\n" +
                "t2 = " + FluxCommonBucket(bucket) +
                $" |> range({TimeRangeToFlux(range)})\n" +
                FluxCommonFilters(device, point) + "\n" +
                "union(tables: [t1, t2])\n" +
                $" |> aggregateWindow(every: duration(v: \"{window}\"), fn: last, createEmpty: false)\n" +
                " |> difference(nonNegative: true)\n" +
                FluxCommonDrop();
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
                if (range.AbsoluteStart != null) {
                    str.Append(range.AbsoluteStartEpoch);
                } else {
                    str.Append("0");
                }
                if (range.AbsoluteStop != null) {
                    str.Append(", stop: ");
                    str.Append(range.AbsoluteStopEpoch);
                }
            } else {
                str.Append("-");
                if (range.RelativeStart != null) {
                    str.Append(range.RelativeStart);
                } else {
                    str.Append("0");
                }
                if (range.RelativeStop != null) {
                    str.Append(", stop: -");
                    str.Append(range.RelativeStop);
                }
            }
            return str.ToString();
        }

    }

}
