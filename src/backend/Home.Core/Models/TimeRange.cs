using System;

namespace Home.Core.Models {

    public class TimeRange {

        public TimeRangeType Type { get; private set; }

        public DateTime AbsoluteStart { get; }
        public DateTime? AbsoluteStop { get; }

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

}
