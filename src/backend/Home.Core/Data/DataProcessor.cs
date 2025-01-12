using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Home.Core.Configuration.Models;
using Home.Core.Interfaces;

namespace Home.Core.Data {

    internal class DataProcessor(ITelemetry telemetry, GlobalConfigurationModel global) {

        public async Task<DataSet> GetData(string deviceId, string point, string since, string toAgo, DateTime? from, DateTime? to, string meanWindow, string diffWindow, string unit) {
            if (string.IsNullOrEmpty(since)) since = "24h";
            TimeRange range;
            IEnumerable<IDataPoint> points;
            if (from != null) {
                range = to != null ? new TimeRange(from.Value, to.Value) : new TimeRange(from.Value);
            } else if (string.IsNullOrEmpty(toAgo)) {
                range = new TimeRange(new RelativeTime(since));
            } else {
                range = new TimeRange(new RelativeTime(since), new RelativeTime(toAgo));
            }
            if (!string.IsNullOrEmpty(diffWindow)) {
                var window = new RelativeTime(diffWindow);
                points = LabelDataWindows(await telemetry.GetWindowDifference(deviceId, point, range, window), window, range);
            } else if (!string.IsNullOrEmpty(meanWindow)) {
                var window = new RelativeTime(meanWindow);
                points = LabelDataWindows(await telemetry.GetWindowMean(deviceId, point, range, window), window, range);
            } else {
                points = await telemetry.GetAllData(deviceId, point, range);
            }
            return new DataSet(deviceId, point, unit, points);
        }

        private IEnumerable<IDataPoint> LabelDataWindows(IEnumerable<IDataPoint> data, RelativeTime window, TimeRange range) {
            List<IDataPoint> points = [];
            var dataPoints = data.ToList();
            var time = dataPoints.First().Time - window;
            var prevTime = DateTime.UnixEpoch;
            while (range.IsInRange(time, window)) {
                points.Insert(0, new LabeledDataPoint(time, 0, window, global, time - window));
                time -= window;
            }
            if (points.Any()) {
                time = points.Last().Time;
                prevTime = time;
            }
            foreach (var point in dataPoints) {
                while (window.CountWindows(time, point.Time) > 1) {
                    time += window;
                    points.Add(new LabeledDataPoint(time, 0, window, global, prevTime));
                    prevTime = time;
                }
                time = point.Time;
                points.Add(new LabeledDataPoint(point, window, global, prevTime));
                prevTime = time;
            }
            time += window;
            while (range.IsInRange(prevTime)) {
                points.Add(new LabeledDataPoint(time, 0, window, global, prevTime));
                prevTime = time;
                time += window;
            }
            return points;
        }

    }

}
