using Home.Core.Interfaces;

namespace Home.Core.Devices {

    public interface ISolarInverter : IDevice {

        double LifeTimeEnergy { get; }
        double LastYearEnergy { get; }
        double LastMonthEnergy { get; }
        double LastDayEnergy { get; }
        
        double CurrentPower { get; }

        double PreviousQuarterEnergy { get; }
        double PreviousQuarterPower { get; }

    }

}
