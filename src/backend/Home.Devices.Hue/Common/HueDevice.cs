using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Home.Core;
using Home.Core.Configuration.Models;
using HueApi;
using HueApi.Models;
using Newtonsoft.Json;

namespace Home.Devices.Hue.Common {

    public abstract class HueDevice : AbstractDevice {
        
        protected readonly LocalHueApi Hue;
        protected Guid HueApiId;

        [JsonIgnore]
        public Guid HueDeviceId { get; private set; } // TODO same as HueApiId?

        protected HueDevice(LocalHueApi hue, Guid hueDeviceId, HomeConfigurationModel home, Device device, ZigbeeConnectivity zigbee, string id) : base(home, id) {
            Hue = hue;
            HueDeviceId = hueDeviceId;
            if (device.Metadata != null) Name = device.Metadata.Name;
            if (zigbee != null) Reachable = zigbee.Status == ConnectivityStatus.connected;
            Manufacturer = device.ProductData.ManufacturerName.HarmonizeManufacturer();
            Model = device.ProductData.ModelId.HarmonizeModel();
            Version = device.ProductData.SoftwareVersion;
            if (string.IsNullOrEmpty(Version) || Version == "0.0.0") Version = Helpers.VersionNotAvailable;
        }

        public void ProcessUpdate(string type, Dictionary<string, JsonElement> data) {
            if (type == "zigbee_connectivity" && data.TryGetValue("status", out JsonElement statusValue)) { 
                var r = statusValue.GetString() == "connected";
                if (Reachable != r) {
                    Reachable = r;
                    NotifyObservers(nameof(Reachable), Reachable);
                }
            }
            if (type == "device_software_update" && data.TryGetValue("state", out JsonElement state)) {
                if (state.GetString() == "no_update") { // update is done
                    Hue.GetDeviceAsync(HueDeviceId).ContinueWith(x => {
                        if (!x.Result.HasErrors) {
                            var newVersion = x.Result.Data.First().ProductData.SoftwareVersion;
                            if (Version != newVersion) {
                                Version = newVersion;
                                NotifyObservers(nameof(Version), Version);
                            }
                        }
                    });
                }
            }
        }
    }

}