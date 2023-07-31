using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using WebApiFunction.Web.AspNet.Controller;

namespace JellyFishBackend.Controller
{
    [Authorize(Policy = "Administrator")]
    [ApiController]
    [Area("jelly-api-1")]
    [Route("[area]/[controller]")]
    public class HealthController : WebApiFunction.Web.AspNet.Controller.HealthController
    {
        public HealthController(HealthCheckService healthCheckService) : base(healthCheckService)
        {
        }

        [HttpGet]
        public override Task<IActionResult> Get()
        {
            return base.Get();
        }
    }
}
