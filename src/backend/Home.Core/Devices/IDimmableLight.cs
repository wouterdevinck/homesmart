using System.Threading.Tasks;

namespace Home.Core.Devices {

    public interface IDimmableLight : IOnOffDevice {

        double Brightness { get; }

        Task SetBrightnessAsync(double bri);

    }

}