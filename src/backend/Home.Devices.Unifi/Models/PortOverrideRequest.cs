using System.Collections.Generic;
using Newtonsoft.Json;

namespace Home.Devices.Unifi.Models {

    public class PortOverrideRequest {

        public PortOverrideRequest(int port, bool poeEnabled) {
            Overrides = new List<Override> { new() { Port = port, PoeMode = poeEnabled ? "auto" : "off"/*, PortPoe  = poeEnabled, NativeNetworkconfId = "65de64909204f15f54fc841e" */} };

            //if (port != 1) {
            //    Overrides.Add(new() { Port = 1, PoeMode = "auto", PortPoe = true, NativeNetworkconfId = "65de64909204f15f54fc841e" });
            //}
            //if (port != 2) {
            //    Overrides.Add(new() { Port = 2, PoeMode = "auto", PortPoe = true, NativeNetworkconfId = "65de64909204f15f54fc841e" });
            //}
            //if (port != 3) {
            //    Overrides.Add(new() { Port = 3, PoeMode = "auto", PortPoe = true, NativeNetworkconfId = "65de64909204f15f54fc841e" });
            //}

        }

        [JsonProperty("port_overrides")]
        public List<Override> Overrides { get; set; }

        public class Override {

            [JsonProperty("port_idx")]
            public int Port { get; set; }

            [JsonProperty("poe_mode")]
            public string PoeMode { get; set; }

            //[JsonProperty("port_poe")]
            //public bool PortPoe { get; set; }

            //[JsonProperty("setting_preference")]
            //public string SettingPreference => "auto";

            //[JsonProperty("name")]
            //public string Name => "PoE Out + Data";

            //[JsonProperty("op_mode")]
            //public string OpMode => "switch";

            //[JsonProperty("port_security_enabled")]
            //public bool PortSecurityEnabled => false;

            //[JsonProperty("port_security_mac_address")]
            //public List<string> PortSecurityMacAddress => new();

            //[JsonProperty("native_networkconf_id")]
            //public string NativeNetworkconfId { get; set; } //"646b87bf68e8c417eb9be4ca"; // TODO

            //[JsonProperty("excluded_networkconf_ids")]
            //public List<string> ExcludedNetworkconfIds => new();

            //[JsonProperty("tagged_vlan_mgmt")]
            //public string TaggedVlanMgmt => "auto";

            //[JsonProperty("stormctrl_bcast_enabled")]
            //public bool StormctrlBcastEnabled => false;

            //[JsonProperty("stormctrl_bcast_rate")]
            //public int StormctrlBcastRate => 100;

            //[JsonProperty("stormctrl_mcast_enabled")]
            //public bool StormctrlMcastEnabled => false;

            //[JsonProperty("stormctrl_mcast_rate")]
            //public int StormctrlMcastRate => 100;

            //[JsonProperty("stormctrl_ucast_enabled")]
            //public bool StormctrlUcastEnabled => false;

            //[JsonProperty("stormctrl_ucast_rate")]
            //public int StormctrlUcastRate => 100;

            //[JsonProperty("egress_rate_limit_kbps_enabled")]
            //public bool EgressRateLimitKbpsEnabled => false;

            //[JsonProperty("autoneg")]
            //public bool Autoneg => true;

            //[JsonProperty("isolation")]
            //public bool Isolation => false;

        }

    }

}