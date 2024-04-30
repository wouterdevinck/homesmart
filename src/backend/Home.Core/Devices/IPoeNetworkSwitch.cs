using System.Threading.Tasks;

namespace Home.Core.Devices {

    public interface IPoeNetworkSwitch : INetworkSwitch {

        Task<bool> TurnPortPowerOnAsync(int port);
        Task<bool> TurnPortPowerOffAsync(int port);

    }

}