using System.Collections.Generic;
using System.Linq;
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

        [HttpGet]
        [Route("automations")]
        public IEnumerable<IAutomation> Automations() {
            return _deviceProvider.GetAutomations().ToList();
        }

    }

}