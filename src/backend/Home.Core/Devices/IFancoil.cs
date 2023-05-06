using System.Threading.Tasks;
using Home.Core.Models;

namespace Home.Core.Devices {

    public interface IFancoil : ITemperatureSensor, IHeater, IOnOffDevice {

        FancoilMode Mode { get; }
        FancoilSpeed Speed { get; }

        Task SetMode(FancoilMode m);
        Task SetSpeed(FancoilSpeed s);

    }

}
