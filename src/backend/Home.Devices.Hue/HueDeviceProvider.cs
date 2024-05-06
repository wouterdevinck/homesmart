using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Home.Core;
using Home.Core.Configuration;
using Home.Core.Configuration.Interfaces;
using Home.Core.Configuration.Models;
using Home.Core.Interfaces;
using Home.Devices.Hue.Devices;
using Microsoft.Extensions.Logging;
using HueApi;
using Home.Devices.Hue.Common;

namespace Home.Devices.Hue {

    public class HueDeviceProvider : AbstractDeviceProvider {

        // TODO test
        //   colortemp correct?
        //   on at full brightness test
        //   commands

        public static Descriptor Descriptor = new("hue", typeof(HueDeviceProvider), typeof(HueConfiguration), DescriptorType.Provider);

        private readonly HomeConfigurationModel _home;
        private readonly ILogger _logger;
        private readonly HueConfiguration _configuration; 
        private readonly List<IDevice> _devices;
        private LocalHueApi _hue;

        public HueDeviceProvider(HomeConfigurationModel home, ILogger logger, IProviderConfiguration configuration) {
            _home = home;
            _logger = logger;
            _configuration = configuration as HueConfiguration;
            _devices = new List<IDevice>();
        }

        public override async Task ConnectAsync() {
            _logger.LogInformation($"Connecting");
            _hue = new LocalHueApi(_configuration.Ip, _configuration.ApiKey);
            _devices.AddRange(await GetAllDevices());
            _logger.LogInformation($"Loaded {_devices.Count()} devices.");
            NotifyObservers(_devices);
            _hue.OnEventStreamMessage += (_, events) => {
                foreach (var hueEvent in events) {
                    if (hueEvent.Type == "update") {
                        foreach (var data in hueEvent.Data) {
                            // TODO Software version updates
                            _logger.LogInformation($"Received event of type {data.Type} for {data.IdV1}.");
                            if (data.Type == "light" || data.Type == "zigbee_connectivity" || data.Type == "device_power" || data.Type == "relative_rotary" || data.Type == "button") {
                                var dev = _devices.SingleOrDefault(x => (x as HueDevice).HueDeviceId == data.Owner.Rid) as HueDevice;
                                if (dev != null) {
                                    var type = dev.GetType();
                                    if (type == typeof(HueDimmableLightDevice)) {
                                        (dev as HueDimmableLightDevice).ProcessUpdate(data.ExtensionData);
                                    } else if (type == typeof(HueColorTemperatureLightDevice)) {
                                        (dev as HueColorTemperatureLightDevice).ProcessUpdate(data.ExtensionData);
                                    } else if (type == typeof(HueExtendedColorLightDevice)) {
                                        (dev as HueExtendedColorLightDevice).ProcessUpdate(data.ExtensionData);
                                    } else if (type == typeof(HueSwitchDevice)) {
                                        (dev as HueSwitchDevice).ProcessUpdate(data.Id, data.ExtensionData);
                                    }
                                }
                            }
                        }
                    }
                }
            };
            _ = _hue.StartEventStream(); // TOOD Reconnect when disconnected?
        }

        public override Task DisconnectAsync() {
            _logger.LogInformation("Disconnecting");
            _hue.StopEventStream();
            _devices.Clear();
            _hue = null;
            return Task.CompletedTask;
        }

        public override IEnumerable<IDevice> GetDevices() {
            return _devices;
        }

        private async Task<List<IDevice>> GetAllDevices() {
            var result = new List<IDevice>();
            var devicesResp = await _hue.GetDevicesAsync();
            var bridgesResp = await _hue.GetBridgeAsync();
            var lightsResp = await _hue.GetLightsAsync();
            var buttonsResp = await _hue.GetButtonsAsync();
            var rotaryResp = await _hue.GetRelativeRotaryAsync();
            var zigbeeResp = await _hue.GetZigbeeConnectivityAsync();
            var powerResp = await _hue.GetDevicePowersAsync();
            if (!devicesResp.HasErrors && !bridgesResp.HasErrors && !lightsResp.HasErrors && !buttonsResp.HasErrors && !rotaryResp.HasErrors && !zigbeeResp.HasErrors && !powerResp.HasErrors) {
                var devices = devicesResp.Data;
                var bridge = bridgesResp.Data.SingleOrDefault();
                var lights = lightsResp.Data;
                var buttons = buttonsResp.Data;
                var rotary = rotaryResp.Data;
                var zigbee = zigbeeResp.Data;
                var power = powerResp.Data;
                foreach (var device in devices) {
                    var types = device.Services.Select(x => x.Rtype).ToList();
                    if (types.Contains("bridge")) {
                        result.Add(new HueBridgeDevice(bridge, device, _hue, _home));
                    } else if (types.Count(x => x == "light") == 1) {
                        var light = lights.SingleOrDefault(x => x.Id == device.Services.Single(x => x.Rtype == "light").Rid);
                        var zgb = zigbee.SingleOrDefault(x => x.Id == device.Services.Single(x => x.Rtype == "zigbee_connectivity").Rid);
                        if (light != null && zgb != null) {
                            if (light.Dimming != null && light.ColorTemperature == null && light.Color == null) {
                                result.Add(new HueDimmableLightDevice(light, device, zgb, _hue, _home));
                            } else if (light.Dimming != null && light.ColorTemperature != null && light.Color == null) {
                                result.Add(new HueColorTemperatureLightDevice(light, device, zgb, _hue, _home));
                            } else if (light.Dimming != null && light.ColorTemperature != null && light.Color != null) {
                                result.Add(new HueExtendedColorLightDevice(light, device, zgb, _hue, _home));
                            }
                        }
                    } else if (types.Contains("button")) {
                        var btns = buttons.Where(x => device.Services.Where(x => x.Rtype == "button").Select(y => y.Rid).Contains(x.Id)).ToList();
                        var rot = rotary.Where(x => device.Services.Where(x => x.Rtype == "relative_rotary").Select(y => y.Rid).Contains(x.Id)).ToList();
                        var zgb = zigbee.SingleOrDefault(x => x.Id == device.Services.Single(x => x.Rtype == "zigbee_connectivity").Rid);
                        var pwr = power.SingleOrDefault(x => x.Id == device.Services.Single(x => x.Rtype == "device_power").Rid);
                        result.Add(new HueSwitchDevice(btns, rot, device, zgb, pwr, _hue, _home));
                    }
                }
            }
            return result;
        }

    }

}
