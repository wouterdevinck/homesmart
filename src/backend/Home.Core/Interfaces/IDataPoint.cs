using System;

namespace Home.Core.Interfaces {

    public interface IDataPoint {

        DateTime Time { get; }
        double Value { get; }

    }

}

