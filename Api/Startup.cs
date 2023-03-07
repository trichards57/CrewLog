using CrewLog.Api;
using CrewLog.Api.Helpers;
using CrewLog.Api.Services;
using CrewLog.Shared.Model;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]

namespace CrewLog.Api
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddLogging();

            builder.Services.AddSingleton(s =>
            {
                var config = s.GetRequiredService<IConfiguration>();
                var client = new CosmosClientBuilder(config.GetConnectionString(Constants.CosmosDbConnectionStringSetting));

                return client.WithConnectionModeDirect()
                    .WithApplicationRegion(Microsoft.Azure.Cosmos.Regions.UKSouth)
                    .WithBulkExecution(true)
                    .Build();
            });
            builder.Services.AddTransient<IServiceBase<Incident>, IncidentService>();
            builder.Services.AddTransient<IServiceBase<Reflection>, ReflectionService>();
            builder.Services.AddTransient<IServiceBase<Role>, RoleService>();
            builder.Services.AddTransient<IServiceBase<Shift>, ShiftService>();
            builder.Services.AddTransient<IServiceBase<Skill>, SkillService>();
        }
    }
}
