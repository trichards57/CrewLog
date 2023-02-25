using Api.Services;
using BlazorApp.Shared.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Api.Functions
{
    public class Incidents
    {
        private readonly IServiceBase<Incident> _incidentsService;
        private readonly ILogger _logger;

        public Incidents(ILoggerFactory loggerFactory, IServiceBase<Incident> incidentsService)
        {
            _logger = loggerFactory.CreateLogger<Incidents>();
            _incidentsService = incidentsService;
        }

        [FunctionName("DeleteIncident")]
        public async Task<IActionResult> Delete(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "incidents/{id:guid}")] HttpRequest req, Guid id, ClaimsPrincipal claimsPrincipal)
        {
            var userIdClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type== ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return new UnauthorizedResult();
            var userId = userIdClaim.Value;

            await _incidentsService.DeleteIfExistsAsync(userId, id);
            return new OkResult();
        }

        [FunctionName("GetIncident")]
        public async Task<IActionResult> Get(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "incidents/{id:guid}")] HttpRequest req, Guid id, ClaimsPrincipal claimsPrincipal)
        {
            var userIdClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type== ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return new UnauthorizedResult();

            var userId = userIdClaim.Value;

            var item = await _incidentsService.GetAsync(userId, id);

            return item != null ? new OkObjectResult(item) : new NotFoundResult();
        }

        [FunctionName("GetAllIncidents")]
        public IActionResult GetAll(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "incidents")] HttpRequest req, ClaimsPrincipal claimsPrincipal)
        {
            var userIdClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type== ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return new UnauthorizedResult();

            var userId = userIdClaim.Value;

            var items = _incidentsService.GetAllAsync(userId);

            return new OkObjectResult(items);
        }

        [FunctionName("PostIncident")]
        public async Task<IActionResult> Post(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "incidents")] HttpRequest req, ClaimsPrincipal claimsPrincipal)
        {
            var userIdClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type== ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return new UnauthorizedResult();
            var userId = userIdClaim.Value;

            var newItemString = await req.ReadAsStringAsync();
            var newItem = System.Text.Json.JsonSerializer.Deserialize<Incident>(newItemString);

            var id = await _incidentsService.UpsertAsync(userId, newItem);

            if (id != null) return new OkObjectResult(await _incidentsService.GetAsync(userId, id.Value));
            return new BadRequestResult();
        }
    }
}
