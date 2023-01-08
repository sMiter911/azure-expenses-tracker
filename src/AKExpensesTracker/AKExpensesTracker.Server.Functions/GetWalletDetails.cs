using System.IO;
using System.Net;
using System.Threading.Tasks;
using AKExpensesTracker.Server.Data.Interfaces;
using AKExpensesTracker.Shared.DTOs;
using AKExpensesTracker.Shared.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace AKExpensesTracker.Server.Functions
{
    public class GetWalletDetails
    {
        private readonly ILogger<GetWalletDetails> _logger;
        private readonly IWalletsRepository _walletRepository;

        public GetWalletDetails(ILogger<GetWalletDetails> log, IWalletsRepository walletRepository)
        {
            _logger = log;
            _walletRepository = walletRepository;
        }

        [FunctionName("GetWalletDetails")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req)
        {
            var userId = "userId";
            string walletId = req.Query["id"];
            _logger.LogInformation($"Rerieve the wallet details for {walletId} id and for user {userId}");

            if (string.IsNullOrEmpty(walletId))
            {
                return new BadRequestObjectResult(new ApiErrorResponse("Please pass a wallet id on the query string"));
            }

            var wallet = await _walletRepository.GetByIdAsync(walletId, userId);
            if (wallet == null)
            {
                return new NotFoundResult();
            }
            else
            {
                return new OkObjectResult(new WalletDTO
                {
                    Id = wallet.Id,
                    Name = wallet.Name,
                    Currency = wallet.Currency,
                    AccountType = wallet.AccountType,
                    Balance = wallet.Balance,
                    BankName = wallet.BankName,
                    Iban = wallet.Iban,
                    CreationDate = wallet.CreationDate,
                    Swift = wallet.Swift,
                    Type = wallet.Type.Value,
                    UserName = wallet.UserName
                });
            }
        }
    }
}

