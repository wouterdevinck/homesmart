using System.Threading.Tasks;
using Home.Core.Interfaces;

namespace Home.Core.Devices {

    public interface IOnOffDevice : IDevice {

        bool On { get; }

        Task ToggleOnOffAsync();
        Task TurnOnAsync();
        Task TurnOffAsync();

    }

}
