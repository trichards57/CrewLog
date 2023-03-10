using CrewLog.Api.Helpers;
using CrewLog.Api.Services;
using CrewLog.Shared.Model;
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
    public class Roles
    {
        private readonly ILogger _logger;
        private readonly IServiceBase<Role> _rolesService;

        public Roles(ILoggerFactory loggerFactory, IServiceBase<Role> rolesService)
        {
            _logger = loggerFactory.CreateLogger<Roles>();
            _rolesService = rolesService;
        }

        [FunctionName("DeleteRole")]
        public async Task<IActionResult> Delete(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "roles/{id:guid}")] HttpRequest req, Guid id, ClaimsPrincipal claimsPrincipal)
        {
            var userIdClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type== ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return new UnauthorizedResult();
            var userId = userIdClaim.Value;

            await _rolesService.DeleteIfExistsAsync(userId, id);
            return new OkResult();
        }

        [FunctionName("GetRole")]
        public async Task<IActionResult> Get(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "roles/{id:guid}")] HttpRequest req, Guid id, ClaimsPrincipal claimsPrincipal)
        {
            var userIdClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type== ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return new UnauthorizedResult();

            var userId = userIdClaim.Value;

            var item = await _rolesService.GetAsync(userId, id);

            return item != default ? new OkObjectResult(item) : new NotFoundResult();
        }

        [FunctionName("GetAllRoles")]
        public IActionResult GetAll(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "roles")] HttpRequest req, ClaimsPrincipal claimsPrincipal)
        {
            var userIdClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type== ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return new UnauthorizedResult();

            var userId = userIdClaim.Value;

            var items = _rolesService.GetAllAsync(userId);

            return new OkObjectResult(items);
        }

        [FunctionName("PostRole")]
        public async Task<IActionResult> Post(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "roles")] HttpRequest req, ClaimsPrincipal claimsPrincipal)
        {
            var userIdClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type== ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return new UnauthorizedResult();

            var userId = userIdClaim.Value;

            var newItemString = await req.ReadAsStringAsync();
            var newItem = await req.GetJsonBody<Role, RoleValidator>();

            if (!newItem.IsValid)
                return newItem.ToBadRequest();

            var id = await _rolesService.UpsertAsync(userId, newItem.Value);

            if (id != null) return new OkObjectResult(await _rolesService.GetAsync(userId, id.Value));

            return new BadRequestResult();
        }
    }
}
