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
    public class Skills
    {
        private readonly ILogger _logger;
        private readonly IServiceBase<Skill> _skillsService;

        public Skills(ILoggerFactory loggerFactory, IServiceBase<Skill> skillsService)
        {
            _logger = loggerFactory.CreateLogger<Skills>();
            _skillsService = skillsService;
        }

        [FunctionName("DeleteSkill")]
        public async Task<IActionResult> Delete(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "skills/{id:guid}")] HttpRequest req, Guid id, ClaimsPrincipal claimsPrincipal)
        {
            var userIdClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type== ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return new UnauthorizedResult();
            var userId = userIdClaim.Value;

            await _skillsService.DeleteIfExistsAsync(userId, id);
            return new OkResult();
        }

        [FunctionName("GetSkill")]
        public async Task<IActionResult> Get(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "skills/{id:guid}")] HttpRequest req, Guid id, ClaimsPrincipal claimsPrincipal)
        {
            var userIdClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type== ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return new UnauthorizedResult();

            var userId = userIdClaim.Value;

            var item = await _skillsService.GetAsync(userId, id);

            return item != null ? new OkObjectResult(item) : new NotFoundResult();
        }

        [FunctionName("GetAllSkills")]
        public IActionResult GetAll(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "skills")] HttpRequest req, ClaimsPrincipal claimsPrincipal)
        {
            var userIdClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type== ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return new UnauthorizedResult();

            var userId = userIdClaim.Value;

            var items = _skillsService.GetAllAsync(userId);

            return new OkObjectResult(items);
        }

        [FunctionName("PostSkill")]
        public async Task<IActionResult> Post(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "skills")] HttpRequest req, ClaimsPrincipal claimsPrincipal)
        {
            var userIdClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type== ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return new UnauthorizedResult();

            var userId = userIdClaim.Value;

            var newItemString = await req.ReadAsStringAsync();
            var newItem = await req.GetJsonBody<Skill, SkillValidator>();

            if (!newItem.IsValid)
                return newItem.ToBadRequest();

            var id = await _skillsService.UpsertAsync(userId, newItem.Value);

            if (id != null) return new OkObjectResult(await _skillsService.GetAsync(userId, id.Value));
            
            return new BadRequestResult();
        }
    }
}
