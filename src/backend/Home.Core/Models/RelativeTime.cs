using System;

namespace Home.Core.Models {

    public class RelativeTime {

        public double Value { get; }
        public TimeUnit Unit { get; }

        public static implicit operator RelativeTime(string time) => new(time);
        public static implicit operator TimeSpan(RelativeTime time) => time.ToTimeSpan();

        public RelativeTime(double value, TimeUnit unit) {
            Value = value;
            Unit = unit;
        }

        public RelativeTime(string str) {
            Value = double.Parse(str.Substring(0, str.Length - 1));
            Unit = StringToTimeUnit(str.Substring(str.Length - 1));
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
                _ => TimeSpan.MinValue
            };
        }

        private string TimeUnitToString(TimeUnit unit) {
            return unit switch {
                TimeUnit.Seconds => "s",
                TimeUnit.Minutes => "m",
                TimeUnit.Hours => "h",
                TimeUnit.Days => "d",
                TimeUnit.Weeks => "w",
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
                _ => TimeUnit.Hours
            };
        }

    }

    public enum TimeUnit {
        Seconds,
        Minutes,
        Hours,
        Days,
        Weeks
    }

    public enum TimeRangeType {
        Absolute,
        Relative
    }

}
