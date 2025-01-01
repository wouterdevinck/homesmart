using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Home.Core;
using Home.Core.Configuration;
using Home.Core.Configuration.Interfaces;
using Home.Core.Interfaces;
using Home.Core.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Home.Automations {

    public class OpsgenieAlarmAutomation : AbstractDeviceConsumer, IAutomation {

        public static Descriptor Descriptor = new("opsgenieAlarm", typeof(OpsgenieAlarmAutomation), typeof(OpsgenieAlarmAutomationConfiguration), DescriptorType.Automation);

        public override string Type => "Opsgenie Alarm";
        public string Description { get; set; }

        private const string ApiUrl = "https://api.eu.opsgenie.com/v2/alerts";

        private readonly ILogger _logger;
        private readonly OpsgenieAlarmAutomationConfiguration _configuration;

        private DateTime? _lastFired;

        public OpsgenieAlarmAutomation(ILogger logger, OpsgenieAlarmAutomationConfiguration configuration)
        : base(new List<string> { configuration.DeviceId }) {
            _logger = logger;
            _configuration = configuration;
        }

        protected override void Start() {
            _logger.LogInformation("Automation starting");
            var device = Devices[_configuration.DeviceId];
            device.DeviceUpdate += async (s, e) => {
                if (e.Retained) return;
                if (e.Property.Equals(_configuration.Property, StringComparison.CurrentCultureIgnoreCase)) { 
                    if (_configuration.Value.Equals(e.Value.ToString(), StringComparison.CurrentCultureIgnoreCase)) {
                        if (_configuration.RateLimit != null) {
                            var now = DateTime.UtcNow;
                            if (_lastFired != null && now - _lastFired < _configuration.RateLimit) {
                                _logger.LogInformation($"Automation: blocking Opsgenie alert '{_configuration.Message}' due to rate limit");
                                return;
                            }
                            _lastFired = now;
                        }
                        _logger.LogInformation($"Automation: sending Opsgenie alert '{_configuration.Message}' to {_configuration.Responder}");
                        var req = new OpsgenieRequest {
                            Message = _configuration.Message,
                            Responder = _configuration.Responder,
                            Priority = _configuration.Priority
                        };
                        var json = JsonConvert.SerializeObject(req);
                        var data = new StringContent(json, Encoding.UTF8, "application/json");
                        using var client = new HttpClient();
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("GenieKey", _configuration.ApiKey);
                        var result = await client.PostAsync(ApiUrl, data);
                        if (!result.IsSuccessStatusCode) {
                            _logger.LogError($"Opsgenie API error: {result.StatusCode} {result.ReasonPhrase}");
                        }
                    }
                }
            };
        }

        private class OpsgenieRequest {

            [JsonProperty("message")]
            public string Message { get; set; }

            [JsonIgnore]
            public string Responder { get; set; }

            [JsonProperty("responders")]
            public IEnumerable<OpsgenieResponder> Responders =>
                new List<OpsgenieResponder> {
                    new OpsgenieResponder {
                        Name = Responder
                    }
                };

            [JsonProperty("priority")]
            public string Priority { get; set; }

            public class OpsgenieResponder {

                [JsonProperty("name")]
                public string Name { get; set; }

                [JsonProperty("type")]
                public string Type => "team";

            }

        }

    }

    public class OpsgenieAlarmAutomationConfiguration : IAutomationConfiguration {

        public string DeviceId { get; set; }
        public string Property { get; set; }
        public string Value { get; set; }
        public string ApiKey { get; set; }
        public string Responder { get; set; }
        public string Priority { get; set; }
        public string Message { get; set; }
        public RelativeTime RateLimit { get; set; }

    }

}
