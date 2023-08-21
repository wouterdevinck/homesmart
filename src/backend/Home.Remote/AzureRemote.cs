using System.Threading.Tasks;
using System;
using Home.Core.Configuration;
using Home.Core.Interfaces;
using Microsoft.Azure.Devices.Client;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using Home.Core.Remote;
using System.Text;
using Newtonsoft.Json.Serialization;
using Microsoft.Azure.SignalR.Management;
using System.Threading;
using Microsoft.AspNetCore.SignalR;

namespace Home.Remote {

    public class AzureRemote : IRemote {

        public static Descriptor Descriptor = new("azure", typeof(AzureRemote), typeof(AzureRemoteConfiguration), DescriptorType.Remote);
        public string Type => "Azure Remote";
        public bool Started { get; private set;  }

        private readonly ILogger _logger;
        private readonly AzureRemoteConfiguration _configuration;
        private readonly HttpClient _http;

        public AzureRemote(ILogger logger, AzureRemoteConfiguration configuration) {
            _logger = logger;
            _configuration = configuration;
            _http = new HttpClient {
                BaseAddress = new Uri(_configuration.ApiUrl)
            };
        }

        public void Install(IDeviceProvider provider) {
            // TODO Error handling e.g. config error or connection error & auto re-connect?

            // Iot
            var deviceAuthentication = new DeviceAuthenticationWithRegistrySymmetricKey(_configuration.Iot.Id, _configuration.Iot.Key);
            var deviceClient = DeviceClient.Create(_configuration.Iot.Host, deviceAuthentication, TransportType.Mqtt);
            deviceClient.SetMethodHandlerAsync("api", ApiHandler, null);

            // Notifications
            var serviceManager = new ServiceManagerBuilder()
                .WithOptions(option => {
                    option.ConnectionString = $"Endpoint=https://{_configuration.Notifications.Host};AccessKey={_configuration.Notifications.Key};Version=1.0;";
                })
                .WithNewtonsoftJson()
                .BuildServiceManager();
            var hub = serviceManager.CreateHubContextAsync("notifications", CancellationToken.None).Result;

            Action<IDevice> subscribe = device => {
                device.DeviceUpdate += async (_, _) => {
                    try {
                        await hub.Clients.All.SendAsync("deviceupdates", device);
                    } catch (Exception ex) {
                        _logger.LogError($"Error while sending notification - {ex.Message}");
                    }
                };
            };
            foreach (var device in provider.GetDevices()) {
                subscribe(device);
            }
            provider.DeviceDiscovered += (_, device) => {
                subscribe(device);
            };

            Started = true;
        }

        private async Task<MethodResponse> ApiHandler(MethodRequest methodRequest, object userContext) {
            try {
                var data = methodRequest.DataAsJson;
                if (!string.IsNullOrEmpty(data)) {
                    var req = JsonConvert.DeserializeObject<RemoteApiRequest>(data);
                    _logger.LogInformation($"Remote API call {req.Method} {req.Path}");
                    if (req != null) {
                        RemoteApiResponse resp = new();
                        HttpResponseMessage apiResp;
                        switch (req.Method) {
                            case "GET":
                                apiResp = await _http.GetAsync(req.Path);
                                break;
                            case "POST":
                                apiResp = await _http.PostAsync(req.Path, new StringContent(req.Body, Encoding.UTF8, "application/json"));
                                break;
                            default:
                                return new MethodResponse(400);
                        }
                        resp.Status = (int)apiResp.StatusCode;
                        resp.Body = await apiResp.Content.ReadAsStringAsync();
                        var respBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(resp, new JsonSerializerSettings {
                            ContractResolver = new CamelCasePropertyNamesContractResolver()
                        }));
                        return new MethodResponse(respBytes, 200);
                    }
                }
                return new MethodResponse(400);
            } catch (Exception ex) {
                _logger.LogError($"Error while handling remote API call - {ex.Message}");
                return new MethodResponse(500);
            }
        }

    }

}
