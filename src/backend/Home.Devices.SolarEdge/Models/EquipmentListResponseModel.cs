using System.Collections.Generic;

namespace Home.Devices.SolarEdge.Models {

    public class EquipmentListResponseModel {

        public ReportersModel Reporters { get; set; }

    }

    public class ReportersModel {

        public int Count { get; set; }
        public List<EquipmentModel> List { get; set; }

    }

    public class EquipmentModel {

        public string Name { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public string SerialNumber { get; set; }
        // public string KWpDC { get; set; }

    }

}
