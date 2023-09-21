using System.Threading.Tasks;
using Home.Core.Interfaces;

namespace Home.Core.Devices {

    public interface IShutter : IDevice {

        int Position { get; }

        Task MoveUpAsync();
        Task MoveDownAsync();
        Task StopAsync();
        Task MoveToTargetAsync(int t);

    }

}
