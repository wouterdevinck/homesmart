using System;
using System.Timers;
using Home.Core;
using Home.Core.Attributes;
using Home.Core.Configuration.Models;
using Home.Core.Devices;
using Home.Devices.SolarEdge.Models;
using Microsoft.Extensions.Logging;

namespace Home.Devices.SolarEdge {

    [Device]
    public partial class SolarEdgeInverterDevice : AbstractDevice, ISolarInverter, IDisposable {

        // Polling settings in minutes
        private const int TimerInterval = 1;   // Run the timer every minute
        private const int UpdateInterval = 15; // Actually call once the API every 15 minutes
        private const int SafetyMargin = 5;    // Minute 5, 20, 35 and 50 of every hour

        private readonly ILogger _logger;
        private readonly SolarEdgeApiClient _api;
        private readonly Timer _timer;

        [DeviceProperty]
        public double LifeTimeEnergy { get; private set; }

        [DeviceProperty]
        public double LastYearEnergy { get; private set; }

        [DeviceProperty]
        public double LastMonthEnergy { get; private set; }

        [DeviceProperty]
        public double LastDayEnergy { get; private set; }

        [DeviceProperty]
        public double CurrentPower { get; private set; }

        [DeviceProperty]
        public DateTime LastSeen { get; protected set; }

        [DeviceProperty]
        public double PreviousQuarterEnergy { get; private set; }

        [DeviceProperty]
        public double PreviousQuarterPower { get; private set; }

        public SolarEdgeInverterDevice(HomeConfigurationModel home, ILogger logger, SolarEdgeApiClient api, EquipmentModel inverter) : base(home, $"SOLAREDGE-{inverter.SerialNumber}") {
            Name = inverter.Name;
            Manufacturer = inverter.Manufacturer;
            Version = Helpers.VersionNotAvailable;
            Model = inverter.Model;
            Type = Helpers.GetTypeString(Helpers.DeviceType.Solar);
            Reachable = true;
            _logger = logger;
            _api = api;
            _timer = new Timer(TimeSpan.FromMinutes(TimerInterval));
            _timer.Elapsed += OnUpdate;
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }

        private async void OnUpdate(object source, ElapsedEventArgs e) {
            if (DateTime.Now.Minute % UpdateInterval == SafetyMargin) { 
                _logger.LogInformation("Quarterly update of solar data started");
                try {
                    _logger.LogInformation("Updating totals");
                    var totals = await _api.GetTotals();
                    if (LifeTimeEnergy != totals.LifeTimeData.Energy) {
                        LifeTimeEnergy = totals.LifeTimeData.Energy;
                        NotifyObservers(nameof(LifeTimeEnergy), LifeTimeEnergy, totals.LastUpdateTime);
                    }
                    if (LastYearEnergy != totals.LastYearData.Energy) {
                        LastYearEnergy = totals.LastYearData.Energy;
                        NotifyObservers(nameof(LastYearEnergy), LastYearEnergy, totals.LastUpdateTime);
                    }
                    if (LastMonthEnergy != totals.LastMonthData.Energy) {
                        LastMonthEnergy = totals.LastMonthData.Energy;
                        NotifyObservers(nameof(LastMonthEnergy), LastMonthEnergy, totals.LastUpdateTime);
                    }
                    if (LastDayEnergy != totals.LastDayData.Energy) {
                        LastDayEnergy = totals.LastDayData.Energy;
                        NotifyObservers(nameof(LastDayEnergy), LastDayEnergy, totals.LastUpdateTime);
                    }
                    if (CurrentPower != totals.CurrentPower.Power) {
                        CurrentPower = totals.CurrentPower.Power;
                        NotifyObservers(nameof(CurrentPower), CurrentPower, totals.LastUpdateTime);
                    }
                    if (LastSeen != totals.LastUpdateTime) {
                        LastSeen = totals.LastUpdateTime;
                        NotifyObservers(nameof(LastSeen), LastSeen, totals.LastUpdateTime);
                    }
                } catch (Exception ex) {
                    _logger.LogError($"Failed to update totals with reason - {ex.Message}");
                }
                try {
                    _logger.LogInformation("Updating energy");
                    var energy = await _api.GetEnergy();
                    var value = energy.Meters[0].Values[0];
                    // if (PreviousQuarterEnergy != value.Value) {
                        PreviousQuarterEnergy = value.Value;
                        NotifyObservers(nameof(PreviousQuarterEnergy), PreviousQuarterEnergy, value.Date);
                    // }
                } catch (Exception ex) {
                    _logger.LogError($"Failed to update energy with reason - {ex.Message}");
                }
                try {
                    _logger.LogInformation("Updating power");
                    var power = await _api.GetPower();
                    var value = power.Meters[0].Values[0];
                    // if (PreviousQuarterPower != value.Value) {
                        PreviousQuarterPower = value.Value;
                        NotifyObservers(nameof(PreviousQuarterPower), PreviousQuarterPower, value.Date);
                    // }
                } catch (Exception ex) {
                    _logger.LogError($"Failed to update power with reason - {ex.Message}");
                }
            }
        }

        public void Dispose() {
            _timer.Stop();
            _timer.Dispose();
        }

    }

}