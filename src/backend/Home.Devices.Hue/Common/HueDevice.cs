using System;
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
        public Guid HueDeviceId { get; private set; }

        protected HueDevice(LocalHueApi hue, Guid hueDeviceId, HomeConfigurationModel home, Device device, ZigbeeConnectivity zigbee, string id) : base(home, id) {
            Hue = hue;
            HueDeviceId = hueDeviceId;
            Name = device.Metadata.Name;
            if (zigbee != null) Reachable = zigbee.Status == ConnectivityStatus.connected;
            Manufacturer = device.ProductData.ManufacturerName.HarmonizeManufacturer();
            Model = device.ProductData.ModelId.HarmonizeModel();
            Version = device.ProductData.SoftwareVersion;
            if (string.IsNullOrEmpty(Version) || Version == "0.0.0") Version = Helpers.VersionNotAvailable;
        }

    }

}