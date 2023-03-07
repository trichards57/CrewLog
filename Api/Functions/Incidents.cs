using CrewLog.Api.Helpers;
using CrewLog.Api.Services;
using CrewLog.Shared.Model;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CrewLog.Api.Functions
{
    public class Incidents
    {
        private readonly IServiceBase<Incident> _incidentsService;
        private readonly ILogger _logger;
        private readonly IServiceBase<Shift> _shiftsService;

        public Incidents(ILoggerFactory loggerFactory, IServiceBase<Incident> incidentsService, IServiceBase<Shift> shiftsService)
        {
            _logger = loggerFactory.CreateLogger<Incidents>();
            _incidentsService = incidentsService;
            _shiftsService = shiftsService;
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

            var newItem = await req.GetJsonBody<Incident, IncidentValidator>();

            if (!await _shiftsService.CheckExistsAsync(userId, newItem.Value.ShiftId))
                newItem.Errors.Add(new ValidationFailure(nameof(newItem.Value.ShiftId), "Shift does not exist."));

            if (!newItem.IsValid)
                return newItem.ToBadRequest();

            var id = await _incidentsService.UpsertAsync(userId, newItem.Value);

            if (id != null) return new OkObjectResult(await _incidentsService.GetAsync(userId, id.Value));

            return new BadRequestResult();
        }
    }
}
