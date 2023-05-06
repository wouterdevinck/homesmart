using System.Collections.Generic;

namespace Home.Devices.Tuya.Models {

    internal class TuyaDpsRequest : TuyaRequest {

        public Dictionary<int, object> Dps { get; set; }

        public TuyaDpsRequest(string deviceId, int dp, object value) : base(deviceId) {
            Dps = new Dictionary<int, object> { { dp, value } };
        }

    }

}
