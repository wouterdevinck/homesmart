using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Home.Devices.Unifi.Notifications {

    public class NotificationModel {

        public bool IsUnifiDevice => Meta.Rc == "ok" && Meta.Message == "unifi-device:sync"; // device:sync client:sync

        [JsonProperty("data")]
        public List<JObject> Data { get; set; }

        [JsonProperty("meta")]
        public MetaModel Meta { get; set; }

    }

    public class MetaModel {

        [JsonProperty("message")]
        public string Message { get; set; }

        //[JsonProperty("product_line")]
        //public string Product { get; set; }

        [JsonProperty("rc")]
        public string Rc { get; set; }

    }

}