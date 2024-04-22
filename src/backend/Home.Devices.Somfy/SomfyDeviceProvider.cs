using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Home.Core;
using Home.Core.Configuration;
using Home.Core.Configuration.Interfaces;
using Home.Core.Configuration.Models;
using Home.Core.Interfaces;
using Home.Core.Transport;
using Home.Devices.Somfy.Devices;
using Home.Devices.Somfy.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Home.Devices.Somfy {

    public class SomfyDeviceProvider : AbstractDeviceProvider {

        public static Descriptor Descriptor = new("somfy", typeof(SomfyDeviceProvider), typeof(SomfyConfiguration), DescriptorType.Provider);

        private readonly List<IDevice> _devices;
        private readonly HomeConfigurationModel _home;
        private readonly ILogger _logger;
        private readonly SomfyConfiguration _configuration;
        private readonly SomfyApiClient _api;
        private ClientWebSocketWrapper _client;

        public SomfyDeviceProvider(HomeConfigurationModel home, ILogger logger, IProviderConfiguration configuration) {
            _devices = new List<IDevice>();
            _home = home;
            _logger = logger;
            _configuration = configuration as SomfyConfiguration;
            _api = new SomfyApiClient(_configuration.Ip);
        }

        public override async Task ConnectAsync() {
            _logger.LogInformation($"Connecting to {_configuration.Ip}");
            try {
                var brVersion = await _api.GetBridgeVersionAsync();
                var shades = await _api.GetShadesAsync();
                var remotes = shades.SelectMany(s => s.Remotes).DistinctBy(s => s.Address);
                _devices.Add(new SomfyBridgeDevice(_home, brVersion));
                foreach (var shade in shades) {
                    _devices.Add(new SomfyShutterDevice(_home, _api, shade));
                }
                foreach (var remote in remotes) {
                    if (shades.Count(x => x.Remotes.Any(y => y.Address == remote.Address)) > 1) {
                        // A bit of a hack, if a remote is paired with more than one shade, assume it is a timer.
                        _devices.Add(new SomfyRemoteDevice(_home, remote.Address));
                    } else {
                        var name = shades.FirstOrDefault(x => x.Remotes.Any(y => y.Address == remote.Address))?.Name;
                        _devices.Add(new SomfyRemoteDevice(_home, name, remote.Address));
                    }
                }
                NotifyObservers(_devices);
                await ConnectWebSocketAsync();
            } catch (Exception ex) {
                _logger.LogError($"Connect error - {ex.Message}");
            }
        }

        private async Task ConnectWebSocketAsync() {
            _client = new ClientWebSocketWrapper(_api.GetWsUri());
            _client.MessageArrived += (msg) => {
                if (msg.StartsWith("42[")) {
                    var ndx = msg.IndexOf(',');
                    var evt = msg.Substring(3, ndx - 3);
                    if (evt == "shadeState" || evt == "shadeCommand") {
                        var json = msg.Substring(ndx + 1, msg.Length - ndx - 2);
                        switch (evt) {
                            case "shadeState":
                                var shd = JsonConvert.DeserializeObject<SomfyShadeModel>(json);
                                _devices.Select(x => x as SomfyShutterDevice).Where(x => x != null).SingleOrDefault(x => x.ShadeId == shd.Id)?.ProcessUpdate(shd);
                                break;
                            case "shadeCommand":
                                var cmd = JsonConvert.DeserializeObject<SomfyCommandModel>(json);
                                if (cmd.IsFromRemote()) {
                                    _devices.Select(x => x as SomfyRemoteDevice).Where(x => x != null).SingleOrDefault(x => x.Address == cmd.Address)?.ProcessEvent(cmd);
                                }
                                break;
                        }
                    }
                }
            };
            _client.ConnectionClosed += async () => {
                _logger.LogInformation("WebSocket closed");
                foreach (var device in _devices) {
                    (device as SomfyDevice)?.UpdateAvailability(false);
                }
                await Task.Delay(5000);
                _logger.LogInformation("WebSocket reconnecting");
                await ConnectWebSocketAsync(); 
            };
            _client.ConnectionError += (ex) => {
                _logger.LogError($"WebSocket error - {ex.Message}");
            };
            try {
                _logger.LogInformation("WebSocket connecting");
                await _client.ConnectAsync();
            } catch (Exception ex) {
                _logger.LogError($"WebSocket connectasync error - {ex.Message}");
            }
            if (_client.IsConnected) {
                _logger.LogInformation("WebSocket connected");
                foreach (var device in _devices) {
                    (device as SomfyDevice)?.UpdateAvailability(true);
                }
            }
        }

        public override async Task DisconnectAsync() {
            _logger.LogInformation("Disconnecting");
            await _client.CloseAsync();
            foreach (var device in _devices) {
                (device as SomfyDevice)?.UpdateAvailability(false);
            }
        }

        public override IEnumerable<IDevice> GetDevices() {
            return _devices;
        }

    }

}