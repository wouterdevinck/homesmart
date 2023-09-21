using System;
using Home.Core;
using Home.Core.Attributes;
using Home.Core.Configuration.Models;
using Home.Core.Devices;
using Home.Devices.Somfy.Models;
using Newtonsoft.Json;

namespace Home.Devices.Somfy.Devices {

    [Device]
    public partial class SomfyRemoteDevice : SomfyDevice, IShutterRemote {

        [JsonIgnore]
        public string Address { get; private set; }

        public SomfyRemoteDevice(HomeConfigurationModel home, string model, string name, string id) : base(home, $"SOMFY-REMOTE-{id}") {
            Name = name;
            Manufacturer = Helpers.Somfy.HarmonizeManufacturer();
            Version = Helpers.VersionNotAvailable;
            Model = model;
            Type = Helpers.GetTypeString(Helpers.DeviceType.Switch);
            Address = id;
        }

        public SomfyRemoteDevice(HomeConfigurationModel home, string name, string id) : this(home, "Smoove Origin RTS", name, id) {}
        public SomfyRemoteDevice(HomeConfigurationModel home, string id) : this(home, "Chronis Smart RTS", "Chronis Smart RTS", id) {}

        public event EventHandler Up;
        public event EventHandler Down;
        public event EventHandler Stop;

        public void ProcessEvent(SomfyCommandModel cmd) {
            switch (cmd.Command) {
                case "Up":
                    Up?.Invoke(this, null);
                    break;
                case "Down":
                    Down?.Invoke(this, null);
                    break;
                case "My":
                    Stop?.Invoke(this, null);
                    break;
            }
        }

    }

}