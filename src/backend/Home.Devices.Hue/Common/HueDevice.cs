using System;
using Home.Core;
using Home.Core.Configuration.Models;
using HueApi;

namespace Home.Devices.Hue.Common {

    public abstract class HueDevice : AbstractDevice {
        
        protected readonly LocalHueApi Hue;
        protected Guid HueApiId;
        public Guid HueDeviceId { get; private set; }

        protected HueDevice(LocalHueApi hue, Guid hueDeviceId, HomeConfigurationModel home, string id) : base(home, id) {
            Hue = hue;
            HueDeviceId = hueDeviceId;
        }

    }

}