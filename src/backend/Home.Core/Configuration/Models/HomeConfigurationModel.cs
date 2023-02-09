using System.Collections.Generic;

namespace Home.Core.Configuration.Models {

    public class HomeConfigurationModel {

        public List<RoomConfigurationModel> Rooms { get; set; }
        public List<DeviceConfigurationModel> Devices { get; set; }

    }

}
