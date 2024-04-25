using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Home.Devices.Unifi.Models {

    public class NotificationModel {

        public bool IsUnifiNetwork => Meta.IsUnifiDeviceSync && Meta.IsNetwork;
        public bool IsUnifiProtect => Meta.IsUnifiDeviceSync && Meta.IsProtect;
        public bool IsClient => Meta.IsClientSync;

        [JsonProperty("data")]
        public List<JObject> Data { get; set; }

        [JsonProperty("meta")]
        public MetaModel Meta { get; set; }

        public IEnumerable<T> GetData<T>() {
            return Data.Select(x => x.ToObject<T>());
        }

    }

    public class MetaModel {

        public bool IsOk => Rc == "ok";

        public bool IsUnifiDeviceSync => IsOk && Message == "unifi-device:sync";
        public bool IsClientSync => IsOk && Message == "client:sync";
        // device:sync
        // session-metadata:sync

        public bool IsNetwork => Product == "NETWORK";
        public bool IsProtect => Product == "PROTECT";

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("product_line")]
        public string Product { get; set; }

        [JsonProperty("rc")]
        public string Rc { get; set; }

    }

}