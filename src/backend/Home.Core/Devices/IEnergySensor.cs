namespace Home.Core.Devices {

    public interface IEnergySensor {

        double Current { get; }
        double Power { get; }
        double Voltage { get; }
        double Energy { get; }

    }

}
