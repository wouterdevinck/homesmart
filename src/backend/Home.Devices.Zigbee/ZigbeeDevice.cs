using System;
using System.Threading.Tasks;
using Home.Core;
using Home.Core.Attributes;
using Home.Core.Configuration.Models;
using Home.Devices.Zigbee.Models;
using Home.Devices.Zigbee.Models.Requests;
using MQTTnet.Extensions.ManagedClient;
using Newtonsoft.Json;

namespace Home.Devices.Zigbee {

    [Device]
    public abstract partial class ZigbeeDevice : AbstractDevice {
        
        private readonly IManagedMqttClient _mqtt;
        protected ZigbeeConfiguration Configuration;

        [DeviceProperty]
        public string PowerSource { get; protected set; }

        [DeviceProperty]
        public DateTime LastSeen { get; protected set; }
        
        protected ZigbeeDevice(HomeConfigurationModel home, DeviceModel model, IManagedMqttClient mqtt, ZigbeeConfiguration configuration) : base(home, $"ZIGBEE-{model.Id}") {
            _mqtt = mqtt;
            Name = model.Name;
            Manufacturer = model.Definition.Manufacturer.HarmonizeManufacturer();
            Version = string.IsNullOrEmpty(model.Version) ? Helpers.VersionNotAvailable : model.Version;
            Model = model.Model;
            PowerSource = model.PowerSource;
            Reachable = true;
            Configuration = configuration;
        }

        public abstract void ProcessZigbeeUpdate(DeviceUpdate update, bool isRetainedUpdate);

        public virtual void UpdateAvailability(bool available) {
            // TODO - Reachable should also update on/off
            //     1. when going unreachable, on = false
            //     2. when going reachable, ask for status? or assume off? depends on power-on behavior
            if (Reachable != available) {
                Reachable = available;
                NotifyObservers(nameof(Reachable), Reachable);
            }
        }

        protected async Task SendRequestAsync(IZigbeeRequest request) {
            var json = JsonConvert.SerializeObject(request);
            await _mqtt.EnqueueAsync($"{Configuration.BaseTopic}/{Name}/set", json);
            // TODO Check if successful & return result
        }

    }

}