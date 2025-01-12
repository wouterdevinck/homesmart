using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Home.Core;
using Home.Core.Configuration;
using Home.Core.Configuration.Models;
using Home.Core.Data;
using Home.Core.Interfaces;
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

        private Dictionary<string, List<string>> _metadata;
        private DateTime _lastMetadataRefresh;

        private readonly string _timezone;

        public InfluxDbTelemetry(ILogger logger, InfluxDbTelemetryConfiguration configuration, GlobalConfigurationModel global) : base(GetDeviceIdList(configuration)) {
            _logger = logger;
            _configuration = configuration;
            _timezone = global.Timezone;
            _client = new InfluxDBClient(_configuration.Url, _configuration.Token);
        }

        protected override void Start() {
            _logger.LogInformation("Telemetry starting");
            foreach(var device in Devices) {
                var properties = _configuration.Data.Where(x => x.DeviceIds.Contains(device.Key)).SelectMany(x => x.Properties);
                device.Value.DeviceUpdate += (s, e) => {
                    if (properties.Any(x => x.Equals(e.Property, StringComparison.OrdinalIgnoreCase)) && !e.Retained) {
                        using var writeApi = _client.GetWriteApi();
                        var point = PointData.Measurement(e.Property) // Note: property name is used here instead of point name, the case is different.
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
            var pointCase = await PointCaseFromMetadata(device, point);
            return await GetData(FluxQuery.AllData(_configuration.Bucket, device, pointCase, range));
        }

        public async Task<IEnumerable<IDataPoint>> GetWindowDifference(string device, string point, TimeRange range, RelativeTime window) {
            var pointCase = await PointCaseFromMetadata(device, point);
            return await GetData(FluxQuery.DiffWindow(_timezone, _configuration.Bucket, device, pointCase, range, window));
        }

        public async Task<IEnumerable<IDataPoint>> GetWindowMean(string device, string point, TimeRange range, RelativeTime window) {
            var pointCase = await PointCaseFromMetadata(device, point);
            return await GetData(FluxQuery.MeanWindow(_timezone, _configuration.Bucket, device, pointCase, range, window));
        }

        public async Task<IEnumerable<DataPointsMetadata>> GetMetadata() {
            if (MetadataNeedsRefresh()) {
                await RefreshMetaData();
            }
            return _metadata.Select(x => new DataPointsMetadata(x.Key, x.Value.Select(y => y.ToLower()).Distinct().ToList())).ToList();
        }

        private async Task RefreshMetaData() {
            var temp = new Dictionary<string, List<string>>();
            var measurements = await GetStrings(FluxQuery.Measurements(_configuration.Bucket));
            foreach (var point in measurements) {
                var devices = await GetStrings(FluxQuery.Devices(_configuration.Bucket, point));
                foreach (var device in devices) {
                    if (!temp.TryAdd(device, [point])) {
                        temp[device].Add(point);
                    }
                }
            }
            _lastMetadataRefresh = DateTime.Now;
            _metadata = temp;
        }

        private bool MetadataNeedsRefresh(string device, string point) {
            return MetadataNeedsRefresh() || _metadata.ContainsKey(device) || _metadata[device].All(x => x != point.ToLower());
        }

        private bool MetadataNeedsRefresh() {
            return _metadata == null || DateTime.Now - _lastMetadataRefresh > TimeSpan.FromDays(1);
        }

        private async Task<string> PointCaseFromMetadata(string device, string point) {
            if (MetadataNeedsRefresh(device, point)) {
                await RefreshMetaData();
            }
            return _metadata[device].First(x => string.Equals(x, point, StringComparison.InvariantCultureIgnoreCase));
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
            var data = new List<IDataPoint>();
            foreach (var table in fluxTables) {
                data.AddRange(table.Records.Select(x => new InfluxDbDataPoint(x)));
            }
            return data;
        }

        private async Task<IEnumerable<string>> GetStrings(string flux) {
            var fluxTables = await _client.GetQueryApi().QueryAsync(flux, _configuration.Organization);
            var table = fluxTables.SingleOrDefault();
            if (table == null) return new List<string>();
            return table.Records.Select(x => x.GetValueByKey("_value").ToString());
        }

    }

}
