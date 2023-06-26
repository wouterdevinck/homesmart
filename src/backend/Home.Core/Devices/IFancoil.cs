using System.Threading.Tasks;
using Home.Core.Models;

namespace Home.Core.Devices {

    public interface IFancoil : IHeater {

        FancoilMode Mode { get; }
        FancoilSpeed Speed { get; }

        Task SetModeAsync(FancoilMode m);
        Task SetSpeedAsync(FancoilSpeed s);

    }

}
