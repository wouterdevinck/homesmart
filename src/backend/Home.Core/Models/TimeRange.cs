using System;

namespace Home.Core.Models {

    public class TimeRange {

        public TimeRangeType Type { get; private set; }

        public DateTime AbsoluteStart { get; private set; }
        public DateTime? AbsoluteStop { get; private set; }

        public long AbsoluteStartEpoch => (new DateTimeOffset(AbsoluteStart)).ToUnixTimeSeconds();
        public long? AbsoluteStopEpoch => AbsoluteStop == null ? null : (new DateTimeOffset((DateTime)AbsoluteStop)).ToUnixTimeSeconds();

        public RelativeTime RelativeStart { get; private set; }
        public RelativeTime RelativeStop { get; private set; }

        public TimeRange(DateTime start, DateTime stop) {
            Type = TimeRangeType.Absolute;
            AbsoluteStart = start;
            AbsoluteStop = stop;
        }

        public TimeRange(DateTime start) {
            Type = TimeRangeType.Absolute;
            AbsoluteStart = start;
            AbsoluteStop = null;
        }

        public TimeRange(RelativeTime start, RelativeTime stop) {
            Type = TimeRangeType.Relative;
            RelativeStart = start;
            RelativeStop = stop;
        }

        public TimeRange(RelativeTime start) {
            Type = TimeRangeType.Relative;
            RelativeStart = start;
            RelativeStop = null;
        }

    }

    public class RelativeTime {

        public double Value { get; private set; }
        public TimeUnit Unit { get; private set; }

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
    }

    public enum TimeRangeType {
        Absolute,
        Relative
    }

}
