using System;
using System.Globalization;
using Home.Core.Configuration.Models;
using Home.Core.Extensions;
using Home.Core.Interfaces;

namespace Home.Core.Data {

    public class LabeledDataPoint(DateTime time, double value, RelativeTime window, GlobalConfigurationModel config, DateTime previousTime) : IDataPoint {

        public DateTime Time { get; } = time;
        public double Value { get; } = value;

        private readonly DateTime _endTime = TimeZoneInfo.ConvertTimeFromUtc(time, config.GetTz());

        public LabeledDataPoint(IDataPoint src, RelativeTime window, GlobalConfigurationModel config, DateTime previousTime) : this(src.Time, src.Value, window, config, previousTime) { }

        public string Label {
            get {
                var refEndTime = _endTime;
                var incomplete = refEndTime < previousTime + window;
                if (window.Unit is TimeUnit.Seconds or TimeUnit.Minutes or TimeUnit.Hours) {
                    var refStartTime = incomplete ? previousTime + new RelativeTime(1, window.Unit) : refEndTime - window;
                    return $"{FormatDateTime(refStartTime)}-{FormatDateTime(refEndTime)}";
                }
                if (refEndTime is { Hour: 0, Minute: 0, Second: 0 }) {
                    refEndTime = _endTime.AddMilliseconds(-1);
                }
                if (window.Value > 1) {
                    var refStartTime = incomplete ? previousTime + new RelativeTime(1, window.Unit) : refEndTime - new RelativeTime(window.Value - 1, window.Unit);
                    return $"{FormatDateTime(refStartTime)}-{FormatDateTime(refEndTime)}";
                }
                return FormatDateTime(refEndTime);
            }
        }

        private string FormatDateTime(DateTime time) {
            return window.Unit switch {
                TimeUnit.Seconds => time.Second.ToString(),
                TimeUnit.Minutes => time.Minute.ToString(),
                TimeUnit.Hours => time.Hour.ToString(),
                TimeUnit.Days => FormatDateTime(time, "dddd"),
                TimeUnit.Weeks => GetIso8601WeekOfYear(time).ToString(),
                TimeUnit.Months => FormatDateTime(time, "MMMM"),
                TimeUnit.Years => FormatDateTime(time, "yyyy"),
                _ => string.Empty
            };
        }

        private string FormatDateTime(DateTime time, string format) {
            return time
                .ToString(format, CultureInfo.CreateSpecificCulture(config.Culture))
                .FirstCharToUpperCase();
        }

        // https://stackoverflow.com/questions/11154673/get-the-correct-week-number-of-a-given-date
        private int GetIso8601WeekOfYear(DateTime time) {
            var day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day is >= DayOfWeek.Monday and <= DayOfWeek.Wednesday) {
                time = time.AddDays(3);
            }
            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }

    }

}