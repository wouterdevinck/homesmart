using System.Collections.Generic;
using System.Linq;

namespace Home.Core.Configuration.Models {

    public class HomeConfigurationModel {

        public List<RoomConfigurationModel> Rooms { get; set; }
        public List<DeviceConfigurationModel> Devices { get; set; }

        public bool CheckConsistency() {
            foreach (var device in Devices) {
                if (!Rooms.Any(x => x.Id == device.RoomId)) {
                    return false;
                }
            }
            return true;
        }

    }

}
