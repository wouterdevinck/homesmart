using System;
using System.Collections.Generic;
using Home.Core;
using Home.Core.Attributes;
using Home.Core.Configuration.Models;
using Home.Devices.Zigbee.Models;
using MQTTnet.Extensions.ManagedClient;

namespace Home.Devices.Zigbee {

    [Device]
    public abstract partial class ZigbeeDevice : AbstractDevice {

        protected IManagedMqttClient Mqtt;

        [DeviceProperty]
        public string PowerSource { get; protected set; }

        [DeviceProperty]
        public DateTime LastSeen { get; protected set; }

        protected ZigbeeConfiguration _configuration;

        public ZigbeeDevice(List<DeviceConfigurationModel> models, DeviceModel model, IManagedMqttClient mqtt, ZigbeeConfiguration configuration) {
            Mqtt = mqtt;
            Name = model.Name;
            DeviceId = $"ZIGBEE-{model.Id}";
            FriendlyId = Helpers.GetFriendlyId(models, DeviceId);
            Manufacturer = model.Definition.Manufacturer.HarmonizeManufacturer();
            Version = model.Version;
            Model = model.Model;
            PowerSource = model.PowerSource;
            Reachable = true;
            _configuration = configuration;
        }

        public abstract void ProcessZigbeeUpdate(DeviceUpdate update);

        public virtual void UpdateAvailability(bool available) {
            // TODO - Reachable should also update on/off
            //     1. when going unreachable, on = false
            //     2. when going reachable, ask for status? or assume off? depends on power-on behavior
            if (Reachable != available) {
                Reachable = available;
                NotifyObservers(nameof(Reachable), Reachable);
            }
        }

    }

}