using System.Threading.Tasks;
using Home.Core;
using Home.Core.Attributes;
using Home.Core.Configuration.Models;
using Home.Core.Devices;
using Home.Devices.Somfy.Models;
using Newtonsoft.Json;

namespace Home.Devices.Somfy.Devices {

    [Device]
    public partial class SomfyShutterDevice : SomfyDevice, IShutter {

        [JsonIgnore]
        public int ShadeId { get; private set; }

        [DeviceProperty<int>(Unit = "%", Min = 0, Max = 100)]
        public int Position { get; private set; }

        private readonly SomfyApiClient _api;

        public SomfyShutterDevice(HomeConfigurationModel home, SomfyApiClient api, SomfyShadeModel model) : base(home, $"SOMFY-SHUTTER-{model.Id}") {
            Name = model.Name;
            Manufacturer = Helpers.Somfy.HarmonizeManufacturer();
            Version = Helpers.VersionNotAvailable;
            Model = "Altus RTS"; // Presumably
            Type = Helpers.GetTypeString(Helpers.DeviceType.Shutter);
            ShadeId = model.Id;
            Position = model.Position;
            _api = api;
        }

        [DeviceCommand]
        public async Task MoveDownAsync() {
            await _api.MoveDownAsync(ShadeId);
        }

        [DeviceCommand]
        public async Task StopAsync() {
            await _api.StopAsync(ShadeId);
        }

        [DeviceCommand]
        public async Task MoveUpAsync() {
            await _api.MoveUpAsync(ShadeId);
        }

        [DeviceCommand]
        public async Task MoveToTargetAsync(int t) {
            await _api.MoveAsync(ShadeId, t);
        }

        public void ProcessUpdate(SomfyShadeModel update) {
            if (Position != update.Position) {
                Position = update.Position;
                NotifyObservers(nameof(Position), Position);
            }
        }

    }

}