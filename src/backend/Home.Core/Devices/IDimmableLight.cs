using System.Threading.Tasks;

namespace Home.Core.Devices {

    public interface IDimmableLight : IOnOffDevice {

        byte Brightness { get; }

        Task SetBrightnessAsync(byte bri);

    }

}