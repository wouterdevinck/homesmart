using System.Threading.Tasks;
using Home.Core.Models;

namespace Home.Core.Devices {

    public interface IColorLight : IDimmableLight {

        ColorXy ColorXy { get; }

        Task SetColorXyAsync(ColorXy c);

    }

}
