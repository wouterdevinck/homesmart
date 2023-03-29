using Home.Core.Interfaces;

namespace Home.Core.Devices {

    public interface ILockable : IDevice {

        bool Locked { get; }

    }

}
