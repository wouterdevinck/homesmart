using Microsoft.AspNetCore.Mvc;

namespace Home.Web.Controllers {

    [Route("status")]
    public class HealthController : Controller {

        [HttpGet]
        [Route("live")]
        public ActionResult Live() {
            return Ok();
        }

    }

}