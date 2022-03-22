using System.Threading.Tasks;

namespace Home.Core.Devices {

    public interface IColorTemperatureLight : IDimmableLight {

        int ColorTemperature { get; }

        Task SetColorTemperature(int ct);

    }

}
