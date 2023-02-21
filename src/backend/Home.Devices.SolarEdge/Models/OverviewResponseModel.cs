using System;

namespace Home.Devices.SolarEdge.Models {

    public class OverviewResponseModel {

        public OverviewModel Overview { get; set; }

    }

    public class OverviewModel {

        public DateTime LastUpdateTime { get; set; }
        public EnergyModel LifeTimeData { get; set; }
        public EnergyModel LastYearData { get; set; }
        public EnergyModel LastMonthData { get; set; }
        public EnergyModel LastDayData { get; set; }
        public PowerModel CurrentPower { get; set; }
        public string MeasuredBy { get; set; }

    }

    public class EnergyModel {

        public double Energy { get; set; }

    }

    public class PowerModel {

        public double Power { get; set; }

    }

}
