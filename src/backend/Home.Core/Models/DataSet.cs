using System.Collections.Generic;
using Home.Core.Interfaces;

namespace Home.Core.Models {

    public class DataSet(string device, string point, string unit, IEnumerable<IDataPoint> points) {

        public string Device { get; } = device;
        public string Point { get; } = point;
        public string Unit { get; } = unit;
        public IEnumerable<IDataPoint> Points { get; } = points;

    }

}
