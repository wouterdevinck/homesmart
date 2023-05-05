namespace Home.Devices.Tuya.Api {

    public class TuyaResponse {

        public TuyaCommand Command { get; }
        public int ReturnCode { get; }
        public string Json { get; }

        internal TuyaResponse(TuyaCommand command, int returnCode, string json) {
            Command = command;
            ReturnCode = returnCode;
            Json = json;
        }

    }

}