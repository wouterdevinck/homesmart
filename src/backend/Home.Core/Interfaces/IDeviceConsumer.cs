namespace Home.Core.Interfaces {

    public interface IDeviceConsumer {
        
        string Type { get; }
        bool Started { get; }

        void Install(IDeviceProvider provider);

    }

}
