using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentModbus;
using Home.Core;
using Home.Core.Configuration.Models;
using Home.Core.Devices;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Home.Devices.Logo {

    public class LogoLightDevice : AbstractDevice, IOnOffDevice {

        [JsonIgnore]
        public int OutputNumber { get; private set; }

        public bool On { get; private set; }
        public bool Reachable { get; private set; }

        private readonly ILogger _logger;
        private readonly ModbusTcpClient _modbusClient;
        private readonly int _switchAddress;
        private readonly int _switchReturnTime;

        private readonly SemaphoreSlim _semaphore = new(1, 1);

        public LogoLightDevice(List<DeviceConfigurationModel> models, ILogger logger, ModbusTcpClient modbusClient, string name, 
          int switchAddress, int outputNumber, int switchReturnTime, bool on) {
            Name = name;
            Manufacturer = "Siemens";
            DeviceId = $"LOGO-LIGHT-{switchAddress}-{outputNumber}";
            FriendlyId = Helpers.GetFriendlyId(models, DeviceId);
            Type = Helpers.GetTypeString(Helpers.DeviceType.Light);
            OutputNumber = outputNumber;
            On = on;
            _logger = logger;
            _modbusClient = modbusClient;
            _switchAddress = switchAddress;
            _switchReturnTime = switchReturnTime;
        }

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

        public async Task TurnOnAsync() {
            _logger.LogInformation($"Switch on device {DeviceId}");
            if (!On) {
                await ToggleOnOffAsync();
            }
        }

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