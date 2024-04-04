using System.Threading.Tasks;

namespace Home.Core.Devices {

    public interface IPoeNetworkSwitch : INetworkSwitch {

        Task TurnPortPowerOnAsync(int port);
        Task TurnPortPowerOffAsync(int port);

    }

}