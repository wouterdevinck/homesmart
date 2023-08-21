namespace Home.Core.Devices {

    public interface ITrv : IHeater, IBatteryDevice, ILockable {

        int ValvePosition { get; }
        bool IsHeating { get; }
        
    }

}
