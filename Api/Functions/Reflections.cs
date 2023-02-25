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
    public class Reflections
    {
        private readonly IServiceBase<Reflection> _reflectionsService;
        private readonly ILogger _logger;

        public Reflections(ILoggerFactory loggerFactory, IServiceBase<Reflection> reflectionsService)
        {
            _logger = loggerFactory.CreateLogger<Reflections>();
            _reflectionsService = reflectionsService;
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

            var newItemString = await req.ReadAsStringAsync();
            var newItem = System.Text.Json.JsonSerializer.Deserialize<Reflection>(newItemString);

            var id = await _reflectionsService.UpsertAsync(userId, newItem);

            if (id != null) return new OkObjectResult(await _reflectionsService.GetAsync(userId, id.Value));
            return new BadRequestResult();
        }
    }
}
