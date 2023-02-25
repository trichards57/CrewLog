using Api;
using Api.Helpers;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

[assembly: FunctionsStartup(typeof(Startup))]

namespace Api
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddLogging();

            var config = (IConfiguration)builder.Services.First(d => d.ServiceType == typeof(IConfiguration)).ImplementationInstance;

            builder.Services.AddSingleton(s =>
            {
                var client = new CosmosClientBuilder(config[Constants.CosmosDbConnectionStringSetting]);

                return client.WithConnectionModeDirect()
                    .WithApplicationRegion(Microsoft.Azure.Cosmos.Regions.UKSouth)
                    .WithBulkExecution(true)
                    .Build();
            });
        }
    }
}
