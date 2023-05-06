using System;

namespace Home.Devices.Tuya.Models {

    internal class TuyaRequest {

        private string _deviceId;

        public TuyaRequest(string deviceId) {
            _deviceId = deviceId;
        }

        public string T => (DateTime.Now - new DateTime(1970, 1, 1)).TotalSeconds.ToString("0");
        public string Uid => _deviceId;
        public string DevId => _deviceId;
        public string GwId => _deviceId;

    }

}