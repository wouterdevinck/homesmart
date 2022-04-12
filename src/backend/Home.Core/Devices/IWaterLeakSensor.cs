using Home.Core.Interfaces;

namespace Home.Core.Devices {

    public interface IWaterLeakSensor : IDevice {

        bool Leak { get;  }

    }

}
