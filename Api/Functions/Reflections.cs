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
    public class Reflections
    {
        private readonly IServiceBase<Incident> _incidentService;
        private readonly ILogger _logger;
        private readonly IServiceBase<Reflection> _reflectionsService;
        private readonly IServiceBase<Shift> _shiftService;

        public Reflections(ILoggerFactory loggerFactory, IServiceBase<Reflection> reflectionsService, IServiceBase<Shift> shiftService, IServiceBase<Incident> incidentService)
        {
            _logger = loggerFactory.CreateLogger<Reflections>();
            _reflectionsService = reflectionsService;
            _shiftService = shiftService;
            _incidentService = incidentService;
        }

        [FunctionName("DeleteReflection")]
        public async Task<IActionResult> Delete(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "reflections/{id:guid}")] HttpRequest req, Guid id, ClaimsPrincipal claimsPrincipal)
        {
            var userIdClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type== ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return new UnauthorizedResult();
            var userId = userIdClaim.Value;

            await _reflectionsService.DeleteIfExistsAsync(userId, id);
            return new OkResult();
        }

        [FunctionName("GetReflection")]
        public async Task<IActionResult> Get(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "reflections/{id:guid}")] HttpRequest req, Guid id, ClaimsPrincipal claimsPrincipal)
        {
            var userIdClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type== ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return new UnauthorizedResult();

            var userId = userIdClaim.Value;

            var item = await _reflectionsService.GetAsync(userId, id);

            return item != null ? new OkObjectResult(item) : new NotFoundResult();
        }

        [FunctionName("GetAllReflections")]
        public IActionResult GetAll(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "reflections")] HttpRequest req, ClaimsPrincipal claimsPrincipal)
        {
            var userIdClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type== ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return new UnauthorizedResult();

            var userId = userIdClaim.Value;

            var items = _reflectionsService.GetAllAsync(userId);

            return new OkObjectResult(items);
        }

        [FunctionName("PostReflection")]
        public async Task<IActionResult> Post(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "reflections")] HttpRequest req, ClaimsPrincipal claimsPrincipal)
        {
            var userIdClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type== ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return new UnauthorizedResult();

            var userId = userIdClaim.Value;

            var newItem = await req.GetJsonBody<Reflection, ReflectionValidator>();

            if (newItem.Value.SourceShiftId.HasValue && !await _shiftService.CheckExistsAsync(userId, newItem.Value.SourceShiftId.Value))
                newItem.Errors.Add(new ValidationFailure(nameof(newItem.Value.SourceShiftId), "Source shift does not exist."));
            if (newItem.Value.SourceIncidentId.HasValue && !await _incidentService.CheckExistsAsync(userId, newItem.Value.SourceIncidentId.Value))
                newItem.Errors.Add(new ValidationFailure(nameof(newItem.Value.SourceIncidentId), "Source incident does not exist."));

            if (!newItem.IsValid)
                return newItem.ToBadRequest();

            var id = await _reflectionsService.UpsertAsync(userId, newItem.Value);

            if (id != null) return new OkObjectResult(await _reflectionsService.GetAsync(userId, id.Value));

            return new BadRequestResult();
        }
    }
}
