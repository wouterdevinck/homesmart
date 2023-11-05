using System;
using Home.Core.Interfaces;

namespace Home.Core.Devices {

    public interface IDimmer : IDevice {

        event EventHandler On;
        event EventHandler Up;
        event EventHandler Down;
        event EventHandler Off;

    }

}
