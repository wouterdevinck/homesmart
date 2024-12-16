namespace Home.Core.Devices {

    public interface IWatermeter {

        int TotalLiters { get; }
        double LitersPerMinute { get; }

    }

}