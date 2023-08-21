using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Home.Core.Interfaces;
using Home.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace Home.Web.Controllers {

    [Route("api/v1")]
    public class ApiController : Controller {

        private readonly ISmartHome _home;

        public ApiController(ISmartHome home) {
            _home = home;
        }

        [HttpGet]
        [Route("devices")]
        public IEnumerable<IDevice> Devices() {
            return _home.GetDevices().ToList();
        }

        [HttpPost]
        [Route("devices/{id}/commands/{command}")]
        public async Task Command(string id, string command, [FromBody] Dictionary<string, object> args) {
            var device = _home.GetDevices().Single(x => x.HasId(id));
            await device.InvokeCommand(command, args);
        }

        [HttpGet]
        [Route("devices/{id}/data/{point}")]
        public async Task<IEnumerable<IDataPoint>> Data(string id, string point, [FromQuery] string since) {
            // TODO Support additional time range parameters
            if (string.IsNullOrEmpty(since)) since = "24h";
            var range = new TimeRange(new RelativeTime(since));
            return await _home.GetDataAsync(id, point, range);
        }

        [HttpGet]
        [Route("providers")]
        public IEnumerable<IDeviceProvider> Providers() {
            return _home.GetProviders().ToList();
        }

        [HttpGet]
        [Route("automations")]
        public IEnumerable<IAutomation> Automations() {
            return _home.GetAutomations().ToList();
        }

        [HttpGet]
        [Route("telemetry")]
        public ITelemetry Telemetry() {
            return _home.GetTelemetry();
        }

        [HttpGet]
        [Route("remote")]
        public IRemote Remote() {
            return _home.GetRemote();
        }

        [HttpGet]
        [Route("rooms")]
        public IEnumerable<IRoom> Rooms() {
            return _home.GetRooms().ToList();
        }

        [HttpGet]
        [Route("rooms/{id}/devices")]
        public IEnumerable<IDevice> RoomsDevices(string id) {
            return _home.GetDevices().Where(x => x.RoomId == id).ToList();
        }

    }

}