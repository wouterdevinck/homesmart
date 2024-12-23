using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Home.Core;
using Home.Core.Configuration;
using Home.Core.Interfaces;
using Home.Core.Models;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using Microsoft.Extensions.Logging;

namespace Home.Telemetry {

    public class InfluxDbTelemetry : AbstractDeviceConsumer, ITelemetry {

        public static Descriptor Descriptor = new("influx", typeof(InfluxDbTelemetry), typeof(InfluxDbTelemetryConfiguration), DescriptorType.Telemetry);

        public override string Type => "InfluxDB Telemetry";

        private readonly ILogger _logger;
        private readonly InfluxDbTelemetryConfiguration _configuration;

        private readonly InfluxDBClient _client;

        public InfluxDbTelemetry(ILogger logger, InfluxDbTelemetryConfiguration configuration) : base(GetDeviceIdList(configuration)) {
            _logger = logger;
            _configuration = configuration;
            _client = new InfluxDBClient(_configuration.Url, _configuration.Token);
        }

        protected override void Start() {
            _logger.LogInformation("Telemetry starting");
            foreach(var device in Devices) {
                var properties = _configuration.Data.Where(x => x.DeviceIds.Contains(device.Key)).SelectMany(x => x.Properties);
                device.Value.DeviceUpdate += (s, e) => {
                    if (properties.Any(x => x.Equals(e.Property, StringComparison.OrdinalIgnoreCase)) && !e.Retained) {
                        using var writeApi = _client.GetWriteApi();
                        var point = PointData.Measurement(e.Property)
                            .Tag("device", device.Key)
                            .Field("value", e.Value)
                            .Timestamp(e.Timestamp.ToUniversalTime(), WritePrecision.Ms);
                        writeApi.WritePoint(point, _configuration.Bucket, _configuration.Organization);
                    }
                };
            }
        }

        private static List<string> GetDeviceIdList(InfluxDbTelemetryConfiguration configuration) {
            return configuration.Data == null ? [] : configuration.Data.SelectMany(p => p.DeviceIds).Distinct().ToList();
        }

        public async Task<IEnumerable<IDataPoint>> GetAllData(string device, string point, TimeRange range) {
            return await GetData(FluxDataQuery(device, point, range));
        }

        public Task<IEnumerable<IDataPoint>> GetWindowDifference(string device, string point, TimeRange range, RelativeTime window) {
            var flux = FluxDataQueryWindow(device, point, range, window) +
                " |> difference(nonNegative: true)\n";
            return GetData(flux);
        }

        public Task<IEnumerable<IDataPoint>> GetWindowMean(string device, string point, TimeRange range, RelativeTime window) {
            var flux = FluxDataQueryWindow(device, point, range, window) +
                " |> yield(name: \"mean\")\n";
            return GetData(flux);
        }

        private async Task<IEnumerable<IDataPoint>> GetData(string flux) {
            var fluxTables = await _client.GetQueryApi().QueryAsync(flux, _configuration.Organization);
            var table = fluxTables.SingleOrDefault();
            if (table == null) return new List<IDataPoint>();
            return table.Records.Select(x => new InfluxDbDataPoint(x));
        }

        private string FluxDataQuery(string device, string point, TimeRange range) {
            return
                "import \"strings\"\n" +
                "import \"timezone\"\n" +
                "option location = timezone.location(name: \"Europe/Brussels\")\n" +
               $"from(bucket:\"{_configuration.Bucket}\")\n" +
               $" |> range({TimeRangeToFlux(range)})\n" +
               $" |> filter(fn: (r) => strings.toLower(v: r[\"_measurement\"]) == \"{point.ToLower()}\")\n" +
                " |> filter(fn: (r) => r[\"_field\"] == \"value\")\n" +
               $" |> filter(fn: (r) => strings.toLower(v: r[\"device\"]) == \"{device.ToLower()}\")\n";
        }

        private string FluxDataQueryWindow(string device, string point, TimeRange range, RelativeTime window) {
            return FluxDataQuery(device, point, range) + 
               $" |> aggregateWindow(every: duration(v: \"{window}\"), fn: last, createEmpty: false)\n";
        }

        // Note: RelativeTime implements a subset of duration https://docs.influxdata.com/flux/v0.x/data-types/basic/duration/
        //    Not all unit specifiers are implemented and neither are combinations of multiple units.

        private static string TimeRangeToFlux(TimeRange range) {
            var str = new StringBuilder("start: ");
            if (range.Type == TimeRangeType.Absolute) {
                str.Append(range.AbsoluteStartEpoch);
                if(range.AbsoluteStop != null) {
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
