using System.Text;
using Home.Core.Extensions;
using Home.Core.Models;

namespace Home.Telemetry {

    internal static class FluxQuery {

        // TODO Return too much data: v1/devices/sensor-living/data/temperature?meanWindow=1h&since=12h

        private static string FluxCommonOptions(string timezone) {
            return
                "import \"timezone\"\n" +
                $"option location = timezone.location(name: \"{timezone}\")\n";
        }

        private static string FluxCommonBucket(string bucket) {
            return $"from(bucket:\"{bucket}\")\n";
        }

        private static string FluxCommonFilters(string device, string point) {
            return
                $" |> filter(fn: (r) => r[\"_measurement\"] == \"{point}\")\n" +
                " |> filter(fn: (r) => r[\"_field\"] == \"value\")\n" +
                $" |> filter(fn: (r) => r[\"device\"] == \"{device}\")\n";
        }

        private static string FluxCommonDrop() {
            return " |> drop(columns: [\"_start\", \"_stop\", \"device\", \"_measurement\", \"_field\"])";
        }

        public static string AllData(string bucket, string device, string point, TimeRange range) {
            return
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

        public static string MeanWindow(string timezone, string bucket, string device, string point, TimeRange range, RelativeTime window) {
            return
                FluxCommonOptions(timezone) +
                FluxCommonBucket(bucket) +
                $" |> range({TimeRangeToFlux(range)})\n" +
                FluxCommonFilters(device, point) +
                $" |> aggregateWindow(every: duration(v: \"{window}\"){OffsetIfNeeded(window)}, fn: mean, createEmpty: false)\n" +
                " |> yield(name: \"mean\")\n" +
                FluxCommonDrop();
        }

        public static string DiffWindow(string timezone, string bucket, string device, string point, TimeRange range, RelativeTime window) {
            return
                FluxCommonOptions(timezone) + "\n" +
                "t1 = " + FluxCommonBucket(bucket) +
                $" |> range({TimeRangeToFlux(range.BeginningOfTimeToStartOfTimeRange())})\n" +
                " |> last()\n" +
                FluxCommonFilters(device, point) + "\n" +
                "t2 = " + FluxCommonBucket(bucket) +
                $" |> range({TimeRangeToFlux(range)})\n" +
                FluxCommonFilters(device, point) + "\n" +
                "union(tables: [t1, t2])\n" +
                $" |> aggregateWindow(every: duration(v: \"{window}\"){OffsetIfNeeded(window)}, fn: last, createEmpty: false)\n" +
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

        // Flux increments weeks from the Unix epoch, which was a Thursday. Because of this, by default, all 1w windows begin on Thursday.
        // Use the offset parameter to shift the start of weekly windows to the desired day of the week. -3d shifts the start of the week to Monday.
        // https://docs.influxdata.com/flux/v0/stdlib/universe/aggregatewindow/#downsample-by-calendar-week-starting-on-monday
        private static string OffsetIfNeeded(RelativeTime window) {
            if (window.Unit == TimeUnit.Weeks) {
                return ", offset: -3d";
            }
            return string.Empty;
        }

    }

}
