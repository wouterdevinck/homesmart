using System;
using Home.Core.Interfaces;

namespace Home.Core.Devices {

    public interface IShutterRemote : IDevice {

        event EventHandler Up;
        event EventHandler Down;
        event EventHandler Stop;

    }

}
