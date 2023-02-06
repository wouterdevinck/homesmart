using System;
using System.Collections.Generic;
using System.Linq;
using Home.Core;
using Home.Core.Configuration;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using Microsoft.Extensions.Logging;

namespace Home.Telemetry {

    public class InfluxDbTelemetry : AbstractTelemetry {

        public static ProviderDescription Descriptor = new("influxdb", ProviderDescriptionType.Telemetry, typeof(InfluxDbTelemetry), typeof(InfluxDbTelemetryConfiguration));

        private const string Bucket = "homesmart";
        private const string Org = "homesmart";

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
                var properties = _configuration.Points.SingleOrDefault(x => x.DeviceId == device.Key).Properties;
                device.Value.DeviceUpdate += (s, e) => {
                    if (properties.Any(x => x.Equals(e.Property, StringComparison.OrdinalIgnoreCase))) {
                        using var writeApi = _client.GetWriteApi();
                        var point = PointData.Measurement(e.Property)
                            .Tag("device", device.Key)
                            .Field("value", e.Value)
                            .Timestamp(DateTime.UtcNow, WritePrecision.Ms);
                        writeApi.WritePoint(point, Bucket, Org);
                    }
                };
            }
        }

        private static List<string> GetDeviceIdList(InfluxDbTelemetryConfiguration configuration) {
            return configuration.Points.Select(p => p.DeviceId).Distinct().ToList();
        }

    }

}
