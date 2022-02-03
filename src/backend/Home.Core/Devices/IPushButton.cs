using System;
using Home.Core.Interfaces;

namespace Home.Core.Devices {

    public interface IPushButton : IDevice {

        event EventHandler SinglePress;

    }

}
