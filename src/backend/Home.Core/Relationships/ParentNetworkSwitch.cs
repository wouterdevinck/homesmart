using Home.Core.Devices;
using Home.Core.Interfaces;

namespace Home.Core.Relationships {

    public class ParentNetworkSwitch : IRelatedDevice<INetworkSwitch> {

        public INetworkSwitch Device { get; set; }

        public int SwitchPort { get; set; }

    }

}
