using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Home.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Home.Web.Controllers {

    [Route("api/v1")]
    public class ApiController : Controller {

        private readonly IDeviceProvider _deviceProvider;

        public ApiController(IDeviceProvider deviceProvider) {
            _deviceProvider = deviceProvider;
        }

        [HttpGet]
        [Route("devices")]
        public IEnumerable<IDevice> Devices() {
            return _deviceProvider.GetDevices().ToList();
        }

        [HttpPost]
        [Route("devices/{id}/commands/{command}")]
        public async Task Command(string id, string command, [FromBody] Dictionary<string, object> args) {
            var device = _deviceProvider.GetDevices().Single(x => x.DeviceId == id);
            await device.InvokeCommand(command, args);
        }

        [Obsolete]
        [HttpGet]
        [Route("automations")]
        public IEnumerable<IDeviceConsumer> Automations() {
            return _deviceProvider.GetDeviceConsumers().ToList();
        }

        [HttpGet]
        [Route("consumers")]
        public IEnumerable<IDeviceConsumer> Consumers() {
            return _deviceProvider.GetDeviceConsumers().ToList();
        }

        [HttpGet]
        [Route("rooms")]
        public IEnumerable<IRoom> Rooms() {
            return _deviceProvider.GetRooms().ToList();
        }

        [HttpGet]
        [Route("rooms/{id}/devices")]
        public IEnumerable<IDevice> RoomsDevices(string id) {
            return _deviceProvider.GetDevices().Where(x => x.RoomId == id).ToList();
        }

    }

}