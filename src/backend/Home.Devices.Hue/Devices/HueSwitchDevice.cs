using System;
using Home.Core;
using Home.Core.Devices;
using Home.Devices.Hue.Common;
using Q42.HueApi;
using Q42.HueApi.Models;

namespace Home.Devices.Hue.Devices {

    public partial class HueSwitchDevice : HueDevice, IBatteryDevice {
        
        public double Battery { get; private set; }
        public int ButtonEvent { get; private set; }
        
        public DateTime SensorUpdate { get; private set; }

        public HueSwitchDevice(Sensor sensor, HueClient hue) : base(hue, sensor.Id) {
            DeviceId = $"HUE-SENSOR-{sensor.UniqueId}";
            Name = sensor.Name;
            Manufacturer = sensor.ManufacturerName.HarmonizeManufacturer();
            Model = sensor.ModelId;
            Version = sensor.SwVersion;
            Type = Helpers.GetTypeString(Helpers.DeviceType.Switch);
            if (sensor.Config.Reachable != null) Reachable = sensor.Config.Reachable.Value;
            if (sensor.Config.Battery != null) Battery = (double)sensor.Config.Battery;
            if (sensor.State.ButtonEvent != null) ButtonEvent = sensor.State.ButtonEvent.Value;
            if (sensor.State.Lastupdated != null) SensorUpdate = sensor.State.Lastupdated.Value;
        }  
        
        //public async Task SetName(string name) {
        //    await Hue.UpdateSensorAsync(LocalId, name);
        //    // TODO Check if success. E.g. name too short 
        //    Name = name;
        //    NotifyObservers("name", Name);
        //}

    }

}