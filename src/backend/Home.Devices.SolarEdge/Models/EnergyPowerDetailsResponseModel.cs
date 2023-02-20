using System;
using System.Collections.Generic;

namespace Home.Devices.SolarEdge.Models {

    public class EnergyDetailsResponseModel {

        public DetailsModel EnergyDetails { get; set; }

    }

    public class PowerDetailsResponseModel {

        public DetailsModel PowerDetails { get; set; }

    }

    public class DetailsModel {

        public string TimeUnit { get; set; }
        public string Unit { get; set; }
        public List<MeterModel> Meters { get; set; }

    }

    public class MeterModel {

        public string Type { get; set; }
        public List<ValueModel> Values { get; set; }

    }


    public class ValueModel {

        public DateTime Date { get; set; }
        public double Value { get; set; }

    }

}
