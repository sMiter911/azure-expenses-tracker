using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AKExpensesTracker.Server.Data;
using Microsoft.Extensions.Configuration;
using AKExpensesTracker.Shared;
using Microsoft.Extensions.DependencyInjection;
using AKExpensesTracker.Server.Functions.Services;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;

[assembly: FunctionsStartup(typeof(AKExpensesTracker.Server.Functions.Startup))]
namespace AKExpensesTracker.Server.Functions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var connectionString = builder.GetContext().Configuration;

            builder.Services.AddCosmosDbClient(connectionString["CosmosDbConnectionString"]);
            builder.Services.AddCosmosDbRepositories();
            builder.Services.AddValidators();

            builder.Services.AddScoped(sp => new ComputerVisionClient(new ApiKeyServiceClientCredentials(connectionString["ComputerVisionApiKey"]))
            {
                Endpoint = connectionString["ComputerVisionEndpoint"]
            });

            builder.Services.AddScoped<IImageAnalyzerService, ImageAnalyzerService>();

            builder.Services.AddScoped<IStorageServices>(sp => new AzureBlobStorageService(connectionString["AzureWebJobsStorage"]));
        }
    }
}
