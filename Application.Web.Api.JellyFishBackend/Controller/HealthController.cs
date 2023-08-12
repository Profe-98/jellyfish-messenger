using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Application.Shared.Kernel.Web.AspNet.Controller;

namespace Application.Web.Api.JellyFishBackend.Controller
{
    [Authorize(Policy = "Administrator")]
    [ApiController]
    [Area("jelly-api-1")]
    [Route("[area]/[controller]")]
    public class HealthController : Application.Shared.Kernel.Web.AspNet.Controller.HealthController
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
