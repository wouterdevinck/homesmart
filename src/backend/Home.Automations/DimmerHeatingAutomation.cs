using System;
using System.Collections.Generic;
using Home.Core;
using Home.Core.Configuration;
using Home.Core.Configuration.Interfaces;
using Home.Core.Devices;
using Home.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace Home.Automations {

    public class DimmerHeatingAutomation : AbstractDeviceConsumer, IAutomation {

        public static Descriptor Descriptor = new("dimmerHeating", typeof(DimmerHeatingAutomation), typeof(DimmerHeatingAutomationConfiguration), DescriptorType.Automation);

        private const int MinTemp = 5;
        private const int MaxTemp = 30;

        public override string Type => "Toggle Button";
        public string Description { get; set; }

        private readonly ILogger _logger;
        private readonly DimmerHeatingAutomationConfiguration _configuration;

        public DimmerHeatingAutomation(ILogger logger, DimmerHeatingAutomationConfiguration configuration)
        : base(new List<string> { configuration.DimmerId, configuration.HeaterId }) {
            _logger = logger;
            _configuration = configuration;
        }

        protected override void Start() {
            _logger.LogInformation("Automation starting");
            var dimmer = Devices[_configuration.DimmerId] as IDimmer;
            var heater = Devices[_configuration.HeaterId] as IHeater;
            if (dimmer != null && heater != null) {

                dimmer.On += async (s, e) => {
                    _logger.LogInformation($"Automation: dimmer {_configuration.DimmerId} turning heater {_configuration.HeaterId} on");
                    await heater.TurnOnAsync();
                };
                dimmer.Off += async (s, e) => {
                    _logger.LogInformation($"Automation: dimmer {_configuration.DimmerId} turning heater {_configuration.HeaterId} off");
                    await heater.TurnOffAsync();
                };
                dimmer.Up += async (s, e) => {
                    var setpoint = Math.Min(heater.RequestedTemperature + 1, MaxTemp);
                    _logger.LogInformation($"Automation: dimmer {_configuration.DimmerId} turning heater {_configuration.HeaterId} up to {setpoint}");
                    await heater.SetRequestedTemperatureAsync(setpoint);
                };
                dimmer.Down += async (s, e) => {
                    var setpoint = Math.Max(heater.RequestedTemperature - 1, MinTemp);
                    _logger.LogInformation($"Automation: dimmer {_configuration.DimmerId} turning heater {_configuration.HeaterId} down to {setpoint}");
                    await heater.SetRequestedTemperatureAsync(setpoint);
                };
            } else {
                _logger.LogError("Automation failed to start");
            }
        }

    }

    public class DimmerHeatingAutomationConfiguration : IAutomationConfiguration {

        public string DimmerId { get; set; }
        public string HeaterId { get; set; }

    }

}
