using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AKExpensesTracker.Server.Data;
using Microsoft.Extensions.Configuration;

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
        }
    }
}
