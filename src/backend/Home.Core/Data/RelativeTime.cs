using System;

namespace Home.Core.Data {

    public class RelativeTime {

        public double Value { get; }
        public TimeUnit Unit { get; }

        public static implicit operator RelativeTime(string time) => new(time);
        public static implicit operator TimeSpan(RelativeTime time) => time.ToTimeSpan();

        // Note: These are for months and years more accurate than the implicit conversion
        // to timespan above, since months and years are not fixed lengths.
        public static DateTime operator +(DateTime a, RelativeTime b) => b.AddToDateTime(a);
        public static DateTime operator -(DateTime a, RelativeTime b) => b.AddToDateTime(a, true);

        public RelativeTime(double value, TimeUnit unit) {
            Value = value;
            Unit = unit;
        }

        public RelativeTime(string str) {
            var unitLength = 1;
            if (str.EndsWith("mo")) {
                unitLength = 2;
            }
            Value = double.Parse(str[..^unitLength]);
            Unit = StringToTimeUnit(str[^unitLength..]);
        }

        public override string ToString() {
            return $"{Value}{TimeUnitToString(Unit)}";
        }

        public TimeSpan ToTimeSpan() {
            return Unit switch {
                TimeUnit.Seconds => TimeSpan.FromSeconds(Value),
                TimeUnit.Minutes => TimeSpan.FromMinutes(Value),
                TimeUnit.Hours => TimeSpan.FromHours(Value),
                TimeUnit.Days => TimeSpan.FromDays(Value),
                TimeUnit.Weeks => TimeSpan.FromDays(Value * 7),
                TimeUnit.Months => TimeSpan.FromDays(Value * 30),
                TimeUnit.Years => TimeSpan.FromDays(Value * 365),
                _ => TimeSpan.MinValue
            };
        }

        public DateTime AddToDateTime(DateTime time, bool subtract = false) {
            var m = subtract ? -1 : 1;
            return Unit switch {
                TimeUnit.Seconds => time.AddSeconds(m * Value),
                TimeUnit.Minutes => time.AddMinutes(m * Value),
                TimeUnit.Hours => time.AddHours(m * Value),
                TimeUnit.Days => time.AddDays(m * Value),
                TimeUnit.Weeks => time.AddDays(m * Value * 7),
                TimeUnit.Months => time.AddMonths(m * (int)Value),
                TimeUnit.Years => time.AddYears(m * (int)Value),
                _ => time
            };
        }

        private string TimeUnitToString(TimeUnit unit) {
            return unit switch {
                TimeUnit.Seconds => "s",
                TimeUnit.Minutes => "m",
                TimeUnit.Hours => "h",
                TimeUnit.Days => "d",
                TimeUnit.Weeks => "w",
                TimeUnit.Months => "mo",
                TimeUnit.Years => "y",
                _ => string.Empty
            };
        }

        private TimeUnit StringToTimeUnit(string str) {
            return str switch {
                "s" => TimeUnit.Seconds,
                "m" => TimeUnit.Minutes,
                "h" => TimeUnit.Hours,
                "d" => TimeUnit.Days,
                "w" => TimeUnit.Weeks,
                "mo" => TimeUnit.Months,
                "y" => TimeUnit.Years,
                _ => TimeUnit.Hours
            };
        }

    }

    public enum TimeUnit {
        Seconds,
        Minutes,
        Hours,
        Days,
        Weeks,
        Months,
        Years
    }

    public enum TimeRangeType {
        Absolute,
        Relative
    }

}
