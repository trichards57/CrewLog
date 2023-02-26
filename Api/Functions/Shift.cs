using Api.Helpers;
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
    public class Shifts
    {
        private readonly IServiceBase<Shift> _shiftsService;
        private readonly ILogger _logger;

        public Shifts(ILoggerFactory loggerFactory, IServiceBase<Shift> shiftsService)
        {
            _logger = loggerFactory.CreateLogger<Shifts>();
            _shiftsService = shiftsService;
        }

        [FunctionName("DeleteShift")]
        public async Task<IActionResult> Delete(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "shifts/{id:guid}")] HttpRequest req, Guid id, ClaimsPrincipal claimsPrincipal)
        {
            var userIdClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type== ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return new UnauthorizedResult();
            var userId = userIdClaim.Value;

            await _shiftsService.DeleteIfExistsAsync(userId, id);
            return new OkResult();
        }

        [FunctionName("GetShift")]
        public async Task<IActionResult> Get(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "shifts/{id:guid}")] HttpRequest req, Guid id, ClaimsPrincipal claimsPrincipal)
        {
            var userIdClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type== ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return new UnauthorizedResult();

            var userId = userIdClaim.Value;

            var item = await _shiftsService.GetAsync(userId, id);

            return item != null ? new OkObjectResult(item) : new NotFoundResult();
        }

        [FunctionName("GetAllShifts")]
        public IActionResult GetAll(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "shifts")] HttpRequest req, ClaimsPrincipal claimsPrincipal)
        {
            var userIdClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type== ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return new UnauthorizedResult();

            var userId = userIdClaim.Value;

            var items = _shiftsService.GetAllAsync(userId);

            return new OkObjectResult(items);
        }

        [FunctionName("PostShift")]
        public async Task<IActionResult> Post(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "shifts")] HttpRequest req, ClaimsPrincipal claimsPrincipal)
        {
            var userIdClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type== ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return new UnauthorizedResult();

            var userId = userIdClaim.Value;

            var newItemString = await req.ReadAsStringAsync();
            var newItem = await req.GetJsonBody<Shift, ShiftValidator>();

            if (!newItem.IsValid)
                return newItem.ToBadRequest();

            var id = await _shiftsService.UpsertAsync(userId, newItem.Value);

            if (id != null) return new OkObjectResult(await _shiftsService.GetAsync(userId, id.Value));
            
            return new BadRequestResult();
        }
    }
}
