using System;
using System.Threading;
using System.Threading.Tasks;
using FluentModbus;
using Home.Core;
using Home.Core.Attributes;
using Home.Core.Configuration.Models;
using Home.Core.Devices;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Home.Devices.Logo {

    [Device]
    public partial class LogoLightDevice : AbstractDevice, IOnOffDevice {

        [JsonIgnore]
        public int OutputNumber { get; private set; }

        [DeviceProperty]
        public bool On { get; private set; }

        private readonly ILogger _logger;
        private readonly ModbusTcpClient _modbusClient;
        private readonly int _switchAddress;
        private readonly int _switchReturnTime;

        private readonly SemaphoreSlim _semaphore = new(1, 1);

        public LogoLightDevice(HomeConfigurationModel home, ILogger logger, ModbusTcpClient modbusClient, string name, 
          int switchAddress, int outputNumber, int switchReturnTime, bool on) : base(home, $"LOGO-LIGHT-{switchAddress}-{outputNumber}") {
            Name = name;
            Manufacturer = "Siemens";
            Version = "1.83.01";
            Model = "LOGO! 230 RCE"; // 6ED1052-1FB08-0BA1
            Type = Helpers.GetTypeString(Helpers.DeviceType.Light);
            OutputNumber = outputNumber;
            On = on;
            _logger = logger;
            _modbusClient = modbusClient;
            _switchAddress = switchAddress;
            _switchReturnTime = switchReturnTime;
        }

        [DeviceCommand]
        public async Task ToggleOnOffAsync() {
            _logger.LogInformation($"Toggle device {DeviceId}");
            await _semaphore.WaitAsync();
            try {
                await _modbusClient.WriteSingleCoilAsync(0xFF, _switchAddress, true);
                await Task.Delay(_switchReturnTime);
                await _modbusClient.WriteSingleCoilAsync(0xFF, _switchAddress, false);
                On = !On;
                NotifyObservers(nameof(On), On);
                // TODO Check for errors & return result?
            } catch (Exception ex) {
                _logger.LogError($"Failed to toggle device {DeviceId} - {ex.Message}");
            } finally {
                _semaphore.Release();
            }
        }

        [DeviceCommand]
        public async Task TurnOnAsync() {
            _logger.LogInformation($"Switch on device {DeviceId}");
            if (!On) {
                await ToggleOnOffAsync();
            }
        }

        [DeviceCommand]
        public async Task TurnOffAsync() {
            _logger.LogInformation($"Switch off device {DeviceId}");
            if (On) {
                await ToggleOnOffAsync();
            }
        }

        public void UpdateState(bool on) {
            if (On != on) {
                _logger.LogInformation($"Device {DeviceId} is now {(on ? "on" : "off")}");
                On = on;
                NotifyObservers(nameof(On), On);
            }
        }

        public void UpdateAvailability(bool reachable) {
            if (Reachable != reachable) {
                _logger.LogInformation($"Device {DeviceId} is now {(reachable ? "reachable" : "unreachable")}");
                Reachable = reachable;
                NotifyObservers(nameof(Reachable), Reachable);
            }
        }

    }

}