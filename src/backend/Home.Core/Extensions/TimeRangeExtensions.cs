using Home.Core.Data;

namespace Home.Core.Extensions {

    public static class TimeRangeExtensions {

        public static TimeRange BeginningOfTimeToStartOfTimeRange(this TimeRange range) {
            if (range.Type == TimeRangeType.Absolute) {
                return new TimeRange(null, range.AbsoluteStart);
            }
            return new TimeRange(null, range.RelativeStart);
        }

    }

}
