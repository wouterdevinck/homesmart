using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Home.Core.Interfaces;
using Home.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace Home.Web.Controllers {

    [Route("api/v1")]
    public class ApiController(ISmartHome home) : Controller {

        [HttpGet]
        [Route("devices")]
        public IEnumerable<IDevice> Devices() {
            return home.GetDevices().ToList();
        }

        [HttpPost]
        [Route("devices/{id}/commands/{command}")]
        public async Task Command(string id, string command, [FromBody] Dictionary<string, object> args) {
            var device = home.GetDevices().Single(x => x.HasId(id));
            await device.InvokeCommand(command, args);
        }

        [HttpGet]
        [Route("devices/{id}/data/{point}")]
        public async Task<IEnumerable<IDataPoint>> AllData(string id, string point, [FromQuery] string since, [FromQuery] string toAgo, [FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] string meanWindow, [FromQuery] string diffWindow) {
            if (string.IsNullOrEmpty(since)) since = "24h";
            TimeRange range;
            if (from != null) {
                if (to != null) {
                    range = new TimeRange(from.Value, to.Value);
                } else {
                    range = new TimeRange(from.Value);
                }
            } else if (string.IsNullOrEmpty(toAgo)) {
                range = new TimeRange(new RelativeTime(since));
            } else {
                range = new TimeRange(new RelativeTime(since), new RelativeTime(toAgo));
            }
            var telemetry = home.GetTelemetry();
            if (!string.IsNullOrEmpty(diffWindow)) {
                return await telemetry.GetWindowDifference(id, point, range, new RelativeTime(diffWindow));
            }
            if (!string.IsNullOrEmpty(meanWindow)) {
                return await telemetry.GetWindowMean(id, point, range, new RelativeTime(meanWindow));
            }
            return await telemetry.GetAllData(id, point, range);
        }

        [HttpGet]
        [Route("providers")]
        public IEnumerable<string> Providers() {
            return home.GetProviders().Select(x => x.GetType().Name).ToList();
        }

        [HttpGet]
        [Route("automations")]
        public IEnumerable<IAutomation> Automations() {
            return home.GetAutomations().ToList();
        }

        [HttpGet]
        [Route("telemetry")]
        public ITelemetry Telemetry() {
            return home.GetTelemetry();
        }

        [HttpGet]
        [Route("telemetry/metadata")]
        public async Task<IEnumerable<DataPointMetadata>> TelemetryMetadata() {
            return await home.GetTelemetry().GetMetadata();
        }

        [HttpGet]
        [Route("remote")]
        public IRemote Remote() {
            return home.GetRemote();
        }

        [HttpGet]
        [Route("rooms")]
        public IEnumerable<IRoom> Rooms() {
            return home.GetRooms().ToList();
        }

        [HttpGet]
        [Route("rooms/{id}/devices")]
        public IEnumerable<IDevice> RoomsDevices(string id) {
            return home.GetDevices().Where(x => x.RoomId == id).ToList();
        }

    }

}