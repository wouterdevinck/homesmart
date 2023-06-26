using System.Threading.Tasks;

namespace Home.Core.Devices {

    public interface IHeater : ITemperatureSensor, IOnOffDevice {

        double RequestedTemperature { get; }

        Task SetRequestedTemperatureAsync(double t);

    }

}
