using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Timers;
using FluentModbus;
using Home.Core;
using Home.Core.Configuration;
using Home.Core.Configuration.Interfaces;
using Home.Core.Configuration.Models;
using Home.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace Home.Devices.Logo {

    public class LogoDeviceProvider : AbstractDeviceProvider {

        public static Descriptor Descriptor = new("logo", typeof(LogoDeviceProvider), typeof(LogoConfiguration), DescriptorType.DeviceProvider);

        private readonly List<LogoLightDevice> _devices;
        private readonly HomeConfigurationModel _home;
        private readonly ILogger _logger;
        private readonly LogoConfiguration _configuration;

        private ModbusTcpClient _modbusClient;
        private Timer _timer;

        private int _numberOfOutputs;

        public LogoDeviceProvider(HomeConfigurationModel home, ILogger logger, IDeviceProviderConfiguration configuration) : base(home) {
            _devices = new List<LogoLightDevice>();
            _home = home;
            _logger = logger;
            _configuration = configuration as LogoConfiguration;
        }

        public override async Task ConnectAsync() {
            if (_configuration == null) {
                throw new Exception("Configuration missing");
            }
            _modbusClient = new ModbusTcpClient();
            await TryConnectAsync();
            _timer = new Timer(_configuration.PollingInterval);
            _timer.Elapsed += OnUpdate;
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }

        private async Task TryConnectAsync() {
            foreach (var device in _devices) {
                device.UpdateAvailability(false);
            }
            _logger.LogInformation($"Connecting to {_configuration.Ip}:{_configuration.Port}");
            try {
                _modbusClient.Connect(new IPEndPoint(IPAddress.Parse(_configuration.Ip), _configuration.Port));
            } catch (Exception ex) {
                _logger.LogError($"Failed to connect with reason - {ex.Message}");
                return;
            }
            var devices = await GetAllDevicesAsync();
            var newDevices = devices.Where(x => _devices.All(y => y.DeviceId != x.DeviceId)).ToList();
            _devices.AddRange(newDevices);
            foreach (var device in _devices) {
                device.UpdateAvailability(true);
            }
            NotifyObservers(_devices);
        }

        public override Task DisconnectAsync() {
            _logger.LogInformation("Disconnecting");
            _timer.Stop();
            _modbusClient.Disconnect();
            return Task.CompletedTask;
        }

        private async Task<List<LogoLightDevice>> GetAllDevicesAsync() {
            _numberOfOutputs = (_configuration.Devices.Max(x => x.OutputNumber) + 3) & ~0x03; // next multiple of 4
            var outputs = await GetOutputs();
            var devices = new List<LogoLightDevice>();
            foreach (var device in _configuration.Devices) { 
                devices.Add(new LogoLightDevice(_home, _logger, _modbusClient, device.Name, device.SwitchAddress, 
                    device.OutputNumber, _configuration.SwitchReturnTime, outputs[device.OutputNumber - 1]));
            }
            return devices;
        }

        public override IEnumerable<IDevice> GetDevices() {
            return _devices;
        }

        private async void OnUpdate(object source, ElapsedEventArgs e) {
            if (!_modbusClient.IsConnected) {
                await TryConnectAsync();
            } else {
                bool[] outputs = null;
                try {
                    outputs = await GetOutputs();
                    _logger.LogInformation($"Got {_numberOfOutputs} outputs");
                } catch (Exception ex) {
                    _logger.LogError($"Failed to get outputs with reason - {ex.Message}");
                    await TryConnectAsync();
                }
                if (outputs != null && outputs.Length == _numberOfOutputs) { 
                    for (var i = 0; i < _numberOfOutputs; ++i) {
                        var device = _devices.SingleOrDefault(x => x.OutputNumber == i + 1);
                        if (device != null) {
                            device.UpdateState(outputs[i]);
                        }
                    }
                }
            }
        }

        private async Task<bool[]> GetOutputs() {
            if (_numberOfOutputs > 8) {
                throw new Exception("More than eight outputs not supported");
            }
            var coils = await _modbusClient.ReadCoilsAsync(0xFF, _configuration.OutputsAddress, _numberOfOutputs);
            var outputs = coils.ToArray().First(); 
            var result = new bool[_numberOfOutputs];
            for (var i = 0; i < _numberOfOutputs; ++i) {
                result[i] = ((outputs >> i) & 1) > 0;
            }
            return result;
        }

    }

}