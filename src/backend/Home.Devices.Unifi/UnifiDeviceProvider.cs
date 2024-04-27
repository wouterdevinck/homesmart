using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Home.Core;
using Home.Core.Configuration;
using Home.Core.Configuration.Interfaces;
using Home.Core.Configuration.Models;
using Home.Core.Devices;
using Home.Core.Interfaces;
using Home.Core.Relationships;
using Home.Devices.Unifi.Devices;
using Home.Devices.Unifi.Models;
using Microsoft.Extensions.Logging;

namespace Home.Devices.Unifi {

    public class UnifiDeviceProvider : AbstractDeviceProvider, IDisposable {

        // TODO before merging
        //  * Re-authenticate when token expires

        public static Descriptor Descriptor = new("unifi", typeof(UnifiDeviceProvider), typeof(UnifiConfiguration), DescriptorType.Provider);

        private readonly List<UnifiDevice> _devices;
        private readonly HomeConfigurationModel _home;
        private readonly ILogger _logger;
        private readonly UnifiConfiguration _configuration;
        private readonly UnifiApiClient _api;

        public UnifiDeviceProvider(HomeConfigurationModel home, ILogger logger, IProviderConfiguration configuration) {
            _devices = new List<UnifiDevice>();
            _home = home;
            _logger = logger;
            _configuration = (UnifiConfiguration)configuration;
            _api = new UnifiApiClient(_configuration.Ip, _configuration.Site, _configuration.Username, _configuration.Password);
        }

        private UnifiDevice NetworkDeviceFactory(NetworkDeviceModel model) {
            return model.Model switch {
                "UDMPRO" => new UnifiConsoleDevice(_home, model),
                "USMINI" => new UnifiNetworkSwitchDevice(_home, model),
                "USM8P60" => new UnifiPoeNetworkSwitchDevice(_home, model, _api),
                "UAL6" => new UnifiAccessPointDevice(_home, model),
                _ => null
            };
        }

        private UnifiDevice ProtectDeviceFactory(ProtectDeviceModel model, IEnumerable<ClientModel> clients) {
            return new UnifiCameraDevice(_home, model, clients.FirstOrDefault());
        }

        public override async Task ConnectAsync() {
            if (_configuration == null) {
                throw new Exception("Configuration missing");
            }
            await TryConnectAsync();
        }

        private async Task TryConnectAsync() {
            _logger.LogInformation($"Connecting to site {_configuration.Site}");
            try {
                if (!await _api.LoginAsync()) throw new Exception("Authentication error");
                var devices = await _api.GetDevicesAsync();
                var clients = await _api.GetClientsAsync();
                var allDevices = devices.NetworkDevices.Select(NetworkDeviceFactory).ToList();
                allDevices.AddRange(devices.ProtectDevices.GroupJoin(clients, x => x.Mac, x => x.Mac, ProtectDeviceFactory));
                foreach (var device in allDevices.Where(protectDevice => !string.IsNullOrEmpty(protectDevice.UplinkMac))) {
                    if (allDevices.SingleOrDefault(x => x.Mac == device.UplinkMac) is INetworkSwitch sw) {
                        device.AddRelatedDevice(new ParentNetworkSwitch {Device = sw, SwitchPort = device.UplinkPort});
                    }
                }
                _devices.AddRange(allDevices);
                NotifyObservers(_devices);
                _api.ConnectionClosed += async () => {
                    _logger.LogInformation("WebSocket closed");
                    await Task.Delay(5000);
                    _logger.LogInformation("WebSocket reconnecting");
                    await _api.ConnectWebSocketAsync();
                };
                _api.ConnectionError += (ex) => {
                    _logger.LogError($"WebSocket error - {ex.Message}");
                };
                _api.Connected += () => {
                    _logger.LogInformation("WebSocket connected");
                };
                _api.NetworkDeviceUpdate += (_, device) => {
                    (_devices.SingleOrDefault(x => (x as UnifiNetworkDevice)?.LocalId == device.Id) as UnifiNetworkDevice)?.ProcessUpdate(device);
                };
                _api.ProtectDeviceUpdate += (_, device) => {
                    (_devices.SingleOrDefault(x => (x as UnifiCameraDevice)?.LocalId == device.Id) as UnifiCameraDevice)?.ProcessUpdate(device);
                };
                _api.ClientDeviceUpdate += (_, client) => {
                    (_devices.SingleOrDefault(x => (x as UnifiCameraDevice)?.Mac == client.Mac) as UnifiCameraDevice)?.ProcessUpdate(client);
                };
                try {
                    _logger.LogInformation("WebSocket connecting");
                    await _api.ConnectWebSocketAsync();
                } catch (Exception ex) {
                    _logger.LogError($"WebSocket connectasync error - {ex.Message}");
                }
            } catch (Exception ex) {
                _logger.LogError($"Failed to connect with reason - {ex.Message}");
            }
        }

        public override async Task DisconnectAsync() {
            _logger.LogInformation("Disconnecting");
            await _api.DisconnectWebSocketAsync();
        }

        public override IEnumerable<IDevice> GetDevices() {
            return _devices;
        }

        public void Dispose() {
            _api.Dispose();
        }

    }

}