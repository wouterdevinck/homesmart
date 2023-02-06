using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using Home.Core;
using Home.Core.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Home.Core.Configuration.Interfaces;

namespace Home.Automations {

    public class OpsgenieAlarmAutomation : AbstractDeviceConsumer {

        public static Descriptor Descriptor = new("opsgenieAlarm", typeof(OpsgenieAlarmAutomation), typeof(OpsgenieAlarmAutomationConfiguration), DescriptorType.DeviceConsumer, DescriptorSubtype.Automation);

        public override string Type => "Opsgenie Alarm";

        private const string ApiUrl = "https://api.eu.opsgenie.com/v2/alerts";

        private readonly ILogger _logger;
        private readonly OpsgenieAlarmAutomationConfiguration _configuration;

        public OpsgenieAlarmAutomation(ILogger logger, OpsgenieAlarmAutomationConfiguration configuration)
        : base(new List<string> { configuration.DeviceId }) {
            _logger = logger;
            _configuration = configuration;
        }

        protected override void Start() {
            _logger.LogInformation("Automation starting");
            var device = Devices[_configuration.DeviceId];
            device.DeviceUpdate += async (s, e) => {
                if (e.Property.Equals(_configuration.Property, StringComparison.CurrentCultureIgnoreCase)) { 
                    if (_configuration.Value.Equals(e.Value.ToString(), StringComparison.CurrentCultureIgnoreCase)) {
                        _logger.LogInformation($"Automation: sending Opsgenie alert '{_configuration.Message}' to {_configuration.Responder}");
                        var req = new OpsgenieRequest {
                            Message = _configuration.Message,
                            Responder = _configuration.Responder
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
            public string Priority => "P1";

            public class OpsgenieResponder {

                [JsonProperty("name")]
                public string Name { get; set; }

                [JsonProperty("type")]
                public string Type => "team";

            }

        }

    }

    public class OpsgenieAlarmAutomationConfiguration : IDeviceConsumerConfiguration {

        public string DeviceId { get; set; }
        public string Property { get; set; }
        public string Value { get; set; }
        public string ApiKey { get; set; }
        public string Responder { get; set; }
        public string Message { get; set; }

    }

}