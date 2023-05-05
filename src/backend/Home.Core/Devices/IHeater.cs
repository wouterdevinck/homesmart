using System.Threading.Tasks;

namespace Home.Core.Devices {

    public interface IHeater {

        double RequestedTemperature { get; }

        Task SetRequestedTemperature(double t);

    }

}
