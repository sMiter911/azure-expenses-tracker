using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using AKExpensesTracker.Server.Data.Interfaces;
using AKExpensesTracker.Shared.Responses;
using System.Linq;
using AKExpensesTracker.Server.Data.Models;
using System.Collections.Generic;
using AKExpensesTracker.Shared.DTOs;

namespace AKExpensesTracker.Server.Functions
{
    public class ListWallets
    {
        private readonly ILogger<ListWallets> _logger;
        private readonly IWalletsRepository _walletsRepository;

        public ListWallets(ILogger<ListWallets> logger, IWalletsRepository walletsRepository)
        {
            _logger = logger;
            _walletsRepository = walletsRepository;
        }

        [FunctionName("ListWallets")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req)
        {

            var userId = "userId";

            var wallets = await _walletsRepository.ListByUserIdAsync(userId);
            var result = wallets.Select(w => new WalletSummaryDTO
            {
                Id = w.Id,
                Name = w.Name,
                Type = w.Type.Value,
                Balance = w.Balance,
                Currency = w.Currency
            });

            return new OkObjectResult(new ApiSuccessResponse<IEnumerable<WalletSummaryDTO>>($"{wallets.Count()} wallets have been retrieved", result)); // Should return 200 OK
        }
    }
}

