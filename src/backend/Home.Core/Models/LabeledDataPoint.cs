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
                if (window.Unit is TimeUnit.Seconds or TimeUnit.Minutes or TimeUnit.Hours) {
                    return $"{FormatDateTime(_refEndTime - window)}-{FormatDateTime(_refEndTime)}";
                }
                if (window.Value > 1) {
                    var refStartTime = _refEndTime - new RelativeTime(window.Value - 1, window.Unit);
                    return $"{FormatDateTime(refStartTime)}-{FormatDateTime(_refEndTime)}";
                }
                return FormatDateTime(_refEndTime);
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