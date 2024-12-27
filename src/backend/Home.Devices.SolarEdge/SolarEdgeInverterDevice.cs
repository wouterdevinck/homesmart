using System;
using System.Threading.Tasks;
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
        private readonly TimeZoneInfo _timezone;

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

        public SolarEdgeInverterDevice(HomeConfigurationModel home, ILogger logger, SolarEdgeApiClient api, EquipmentModel inverter, TimeZoneInfo timezone) : base(home, $"SOLAREDGE-{inverter.SerialNumber}") {
            Name = inverter.Name;
            Manufacturer = inverter.Manufacturer;
            Version = Helpers.VersionNotAvailable;
            Model = inverter.Model;
            Type = Helpers.GetTypeString(Helpers.DeviceType.Solar);
            Reachable = true;
            _timezone = timezone;
            _logger = logger;
            _api = api;
            _timer = new Timer(TimeSpan.FromMinutes(TimerInterval));
            _timer.Elapsed += OnUpdate;
            _timer.AutoReset = true;
            _timer.Enabled = true;
            Task.Run(async () => await GetData(true));
        }

        private async void OnUpdate(object source, ElapsedEventArgs e) {
            if (DateTime.Now.Minute % UpdateInterval == SafetyMargin) { 
                _logger.LogInformation("Quarterly update of solar data started");
                await GetData(false);
            }
        }

        private async Task GetData(bool isInitialRun) {
            try {
                _logger.LogInformation("Updating totals");
                var totals = await _api.GetTotals();
                var timeInUtc = TimeZoneInfo.ConvertTimeToUtc(totals.LastUpdateTime, _timezone);
                if (Math.Abs(LifeTimeEnergy - totals.LifeTimeData.Energy) >= Tolerance) {
                    LifeTimeEnergy = totals.LifeTimeData.Energy;
                    NotifyObservers(nameof(LifeTimeEnergy), LifeTimeEnergy, timeInUtc, isInitialRun);
                }
                if (Math.Abs(LastYearEnergy - totals.LastYearData.Energy) >= Tolerance) {
                    LastYearEnergy = totals.LastYearData.Energy;
                    NotifyObservers(nameof(LastYearEnergy), LastYearEnergy, timeInUtc, isInitialRun);
                }
                if (Math.Abs(LastMonthEnergy - totals.LastMonthData.Energy) >= Tolerance) {
                    LastMonthEnergy = totals.LastMonthData.Energy;
                    NotifyObservers(nameof(LastMonthEnergy), LastMonthEnergy, timeInUtc, isInitialRun);
                }
                if (Math.Abs(LastDayEnergy - totals.LastDayData.Energy) >= Tolerance) {
                    LastDayEnergy = totals.LastDayData.Energy;
                    NotifyObservers(nameof(LastDayEnergy), LastDayEnergy, timeInUtc, isInitialRun);
                }
                if (Math.Abs(CurrentPower - totals.CurrentPower.Power) >= Tolerance || (totals.CurrentPower.Power == 0 && CurrentPower > 0)) {
                    CurrentPower = totals.CurrentPower.Power;
                    NotifyObservers(nameof(CurrentPower), CurrentPower, timeInUtc, isInitialRun);
                }
                if (LastSeen != totals.LastUpdateTime) {
                    LastSeen = totals.LastUpdateTime;
                    NotifyObservers(nameof(LastSeen), LastSeen, timeInUtc, isInitialRun);
                }
            } catch (Exception ex) {
                _logger.LogError($"Failed to update totals with reason - {ex.Message}");
            }
            try {
                _logger.LogInformation("Updating energy");
                var energy = await _api.GetEnergy(_timezone);
                var value = energy.Meters[0].Values[0];
                var timeInUtc = TimeZoneInfo.ConvertTimeToUtc(value.Date, _timezone);
                if (Math.Abs(PreviousQuarterEnergy - value.Value) >= Tolerance || (value.Value == 0 && PreviousQuarterEnergy > 0)) {
                    PreviousQuarterEnergy = value.Value;
                    NotifyObservers(nameof(PreviousQuarterEnergy), PreviousQuarterEnergy, timeInUtc, isInitialRun);
                }
            } catch (Exception ex) {
                _logger.LogError($"Failed to update energy with reason - {ex.Message}");
            }
            try {
                _logger.LogInformation("Updating power");
                var power = await _api.GetPower(_timezone);
                var value = power.Meters[0].Values[0];
                var timeInUtc = TimeZoneInfo.ConvertTimeToUtc(value.Date, _timezone);
                if (Math.Abs(PreviousQuarterPower - value.Value) >= Tolerance || (value.Value == 0 && PreviousQuarterPower > 0)) {
                    PreviousQuarterPower = value.Value;
                    NotifyObservers(nameof(PreviousQuarterPower), PreviousQuarterPower, timeInUtc, isInitialRun);
                }
            } catch (Exception ex) {
                _logger.LogError($"Failed to update power with reason - {ex.Message}");
            }
        }

        public void Dispose() {
            _timer.Stop();
            _timer.Dispose();
        }

    }

}