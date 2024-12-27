using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                        var point = PointData.Measurement(e.Property) // BUG: This should be the point name, not the property name. Case is different.
                            .Tag("device", device.Key)
                            .Field("value", e.Value)
                            .Timestamp(e.Timestamp.ToUniversalTime(), WritePrecision.Ms); // BUG: Timezone issues.
                        writeApi.WritePoint(point, _configuration.Bucket, _configuration.Organization);
                    }
                };
            }
        }

        private static List<string> GetDeviceIdList(InfluxDbTelemetryConfiguration configuration) {
            return configuration.Data == null ? [] : configuration.Data.SelectMany(p => p.DeviceIds).Distinct().ToList();
        }

        public async Task<IEnumerable<IDataPoint>> GetAllData(string device, string point, TimeRange range) {
            return await GetData(FluxQuery.AllData(_configuration.Bucket, device, point, range));
        }

        public async Task<IEnumerable<IDataPoint>> GetWindowDifference(string device, string point, TimeRange range, RelativeTime window) {
            return await GetData(FluxQuery.DiffWindow(_configuration.Bucket, device, point, range, window));
        }

        public async Task<IEnumerable<IDataPoint>> GetWindowMean(string device, string point, TimeRange range, RelativeTime window) {
            return await GetData(FluxQuery.MeanWindow(_configuration.Bucket, device, point, range, window));
        }

        public async Task<IEnumerable<DataPointMetadata>> GetMetadata() {
            var metadata = new List<DataPointMetadata>();
            var measurements = await GetStrings(FluxQuery.Measurements(_configuration.Bucket));
            foreach (var measurement in measurements) {
                var devices = await GetStrings(FluxQuery.Devices(_configuration.Bucket, measurement));
                metadata.AddRange(devices.Select(x => new DataPointMetadata(x, measurement)));
            }
            return metadata;
        }

        public async Task ExportCsv(string path) {
            var flux = FluxQuery.AllData(_configuration.Bucket);
            var fluxTables = await _client.GetQueryApi().QueryAsync(flux, _configuration.Organization);
            await using var file = new StreamWriter(path);
            await file.WriteLineAsync("sep=;");
            await file.WriteLineAsync("Time;Device;Point;Value");
            foreach (var record in fluxTables.SelectMany(table => table.Records)) {
                await file.WriteLineAsync($"{record.GetTime()};{record.GetValueByKey("device")};{record.GetValueByKey("_measurement")};{record.GetValueByKey("_value")}");
            }
        }

        private async Task<IEnumerable<IDataPoint>> GetData(string flux) {
            var fluxTables = await _client.GetQueryApi().QueryAsync(flux, _configuration.Organization);
            var table = fluxTables.SingleOrDefault();
            if (table == null) return new List<IDataPoint>();
            return table.Records.Select(x => new InfluxDbDataPoint(x));
        }

        private async Task<IEnumerable<string>> GetStrings(string flux) {
            var fluxTables = await _client.GetQueryApi().QueryAsync(flux, _configuration.Organization);
            var table = fluxTables.SingleOrDefault();
            if (table == null) return new List<string>();
            return table.Records.Select(x => x.GetValueByKey("_value").ToString());
        }

    }

}
