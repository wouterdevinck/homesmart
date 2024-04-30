namespace Home.Core.Interfaces {

    public interface IRelatedDevice<out T> where T : IDevice {

        T Device { get; }

    }

}

