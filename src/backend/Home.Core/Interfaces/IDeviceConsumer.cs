namespace Home.Core.Interfaces {

    public interface IDeviceConsumer {

        string Description { get; set; }
        string Type { get; }
        bool Started { get; }

        void Install(IDeviceProvider provider);

    }

}
