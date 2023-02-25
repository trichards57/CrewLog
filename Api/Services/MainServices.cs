using Api.Helpers;
using BlazorApp.Shared.Model;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;

namespace Api.Services
{
    internal class IncidentService : ServiceBase<Incident>
    {
        public IncidentService(ILogger<IncidentService> logger, CosmosClient cosmosClient)
            : base(logger, cosmosClient, Constants.CosmosDb, Constants.IncidentsContainer)
        {
        }
    }

    internal class ReflectionService : ServiceBase<Reflection>
    {
        public ReflectionService(ILogger<ReflectionService> logger, CosmosClient cosmosClient)
            : base(logger, cosmosClient, Constants.CosmosDb, Constants.ReflectionsContainer)
        {
        }
    }

    internal class RoleService : ServiceBase<Role>
    {
        public RoleService(ILogger<RoleService> logger, CosmosClient cosmosClient)
            : base(logger, cosmosClient, Constants.CosmosDb, Constants.RolesContainer)
        {
        }
    }

    internal class ShiftService : ServiceBase<Shift>
    {
        public ShiftService(ILogger<ShiftService> logger, CosmosClient cosmosClient)
            : base(logger, cosmosClient, Constants.CosmosDb, Constants.ShiftsContainer)
        {
        }
    }

    internal class SkillService : ServiceBase<Skill>
    {
        public SkillService(ILogger<SkillService> logger, CosmosClient cosmosClient)
            : base(logger, cosmosClient, Constants.CosmosDb, Constants.SkillsContainer)
        {
        }
    }
}
