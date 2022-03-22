using Home.Core.Interfaces;

namespace Home.Core.Devices {

    public interface IBatteryDevice : IDevice {

        double Battery { get; }

    }

}
