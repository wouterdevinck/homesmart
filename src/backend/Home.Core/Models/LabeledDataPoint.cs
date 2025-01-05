using System;
using System.Globalization;
using Home.Core.Configuration.Models;
using Home.Core.Extensions;
using Home.Core.Interfaces;

namespace Home.Core.Models {

    public class LabeledDataPoint(IDataPoint src, RelativeTime window, GlobalConfigurationModel config) : IDataPoint {

        public DateTime Time { get; } = src.Time;
        public double Value { get; } = src.Value;

        private readonly DateTime _refEndTime = TimeZoneInfo.ConvertTimeFromUtc(src.Time, config.GetTz()).AddMilliseconds(-1);

        public string Label {
            get {
                var def = _refEndTime.ToString(CultureInfo.InvariantCulture);
                if (window.Value > 1) {
                    return def; // TODO: Implement range labels
                }
                return window.Unit switch {
                    TimeUnit.Seconds => TimeLabelRange(_refEndTime.Second),
                    TimeUnit.Minutes => TimeLabelRange(_refEndTime.Minute),
                    TimeUnit.Hours => TimeLabelRange(_refEndTime.Hour),
                    TimeUnit.Days => TimeLabel("dddd"),
                    TimeUnit.Weeks => GetIso8601WeekOfYear().ToString(),
                    TimeUnit.Months => TimeLabel("MMMM"),
                    TimeUnit.Years => TimeLabel("yyyy"),
                    _ => def
                };
            }
        }

        private string TimeLabel(string format) {
            return _refEndTime
                .ToString(format, CultureInfo.CreateSpecificCulture(config.Culture))
                .FirstCharToUpperCase();
        }

        private string TimeLabelRange(int start) {
            return $"{start}-{start + 1}";
        }

        // https://stackoverflow.com/questions/11154673/get-the-correct-week-number-of-a-given-date
        private int GetIso8601WeekOfYear() {
            var time = _refEndTime;
            var day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day is >= DayOfWeek.Monday and <= DayOfWeek.Wednesday) {
                time = time.AddDays(3);
            }
            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }

    }

}