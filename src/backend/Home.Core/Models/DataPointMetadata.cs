namespace Home.Core.Models {

    public class DataPointMetadata(string deviceId, string point) {

        public string DeviceId { get; } = deviceId;
        public string Point { get; } = point;

    }

}