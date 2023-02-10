using Home.Core.Interfaces;

namespace Home.Core.Configuration.Models {

    public class RoomConfigurationModel : IRoom {

        public string Id { get; set; }
        public string Name { get; set; }

    }

}
