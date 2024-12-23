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
            switch (Unit) {
                case TimeUnit.Seconds:
                    return TimeSpan.FromSeconds(Value);
                case TimeUnit.Minutes:
                    return TimeSpan.FromMinutes(Value);
                case TimeUnit.Hours:
                    return TimeSpan.FromHours(Value);
                case TimeUnit.Days:
                    return TimeSpan.FromDays(Value);
                case TimeUnit.Weeks:
                    return TimeSpan.FromDays(Value * 7);
                default:
                    return TimeSpan.MinValue;
            }
        }

        private string TimeUnitToString(TimeUnit unit) {
            switch (unit) {
                case TimeUnit.Seconds:
                    return "s";
                case TimeUnit.Minutes:
                    return "m";
                case TimeUnit.Hours:
                    return "h";
                case TimeUnit.Days:
                    return "d";
                case TimeUnit.Weeks:
                    return "w";
                default:
                    return string.Empty;
            }
        }

        private TimeUnit StringToTimeUnit(string str) {
            switch (str) {
                case "s":
                    return TimeUnit.Seconds;
                case "m":
                    return TimeUnit.Minutes;
                case "h":
                    return TimeUnit.Hours;
                case "d":
                    return TimeUnit.Days;
                case "w":
                    return TimeUnit.Weeks;
                default:
                    return TimeUnit.Hours;
            }
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
