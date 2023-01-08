using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AKExpensesTracker.Server.Data.Interfaces;
using AKExpensesTracker.Server.Data.Models;
using AKExpensesTracker.Shared.DTOs;
using AKExpensesTracker.Shared.Enum;
using AKExpensesTracker.Shared.Responses;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace AKExpensesTracker.Server.Functions
{
    public class UpsertWallet
    {
        private readonly ILogger<UpsertWallet> _logger;
        private readonly IWalletsRepository _walletsRepository;
        private readonly IValidator<WalletDTO> _walletValidator;

        public UpsertWallet(ILogger<UpsertWallet> log, IWalletsRepository walletsRepository, IValidator<WalletDTO> validator)
        {
            _logger = log;
            _walletsRepository = walletsRepository;
            _walletValidator = validator;
        }

        [FunctionName("UpsertWallet")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("Upsert Wallet Started");
            var userId = "userId";

            // Read the string from the request body (JSON)
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            // Deserialize the JSON into a Wallet object
            var data = JsonConvert.DeserializeObject<WalletDTO>(requestBody);

            // Input Validation
            var validationResult = _walletValidator.Validate(data);
            if (!validationResult.IsValid)
            {
                _logger.LogError("ERROR: Invalid Input");
                return new BadRequestObjectResult(new ApiErrorResponse("Wallet inputs are not valid", validationResult.Errors.Select(e => e.ErrorMessage)));
            }

            // TODO: Validate the number of wallets based on subscription type
            var isAdd = string.IsNullOrWhiteSpace(data.Id);
            Wallet wallet = null;
            // Add operation
            if (isAdd)
            {
                wallet = new()
                {
                    CreationDate = DateTime.UtcNow,
                    Id = Guid.NewGuid().ToString()
                };
            }
            else // Update operation
            {
                wallet = await _walletsRepository.GetByIdAsync(data.Id, userId);
                if (wallet == null)
                {
                    _logger.LogError("ERROR: Wallet not found");
                    return new NotFoundObjectResult(new ApiErrorResponse("Wallet not found"));
                }
            }

            wallet.Name = data.Name;
            wallet.AccountType = data.AccountType;
            wallet.BankName = data.BankName;
            wallet.ModificationDate = DateTime.UtcNow;
            wallet.UserId = userId;
            wallet.UserName = data.UserName;
            wallet.WalletType = data.Type.ToString();
            wallet.Currency = data.Currency;
            wallet.Swift = data.Swift;
            wallet.Iban = data.Iban;

            if (isAdd)
            {
                await _walletsRepository.CreateAsync(wallet);
            } else
            {
                await _walletsRepository.UpdateAsync(wallet);            
            }

            // Set Auto Generated Properties
            data.Id = wallet.Id;
            data.CreationDate = wallet.CreationDate;

            return new OkObjectResult(new ApiSuccessResponse<WalletDTO>($"Wallet has been {(isAdd ? "added" : "updated")} successfully", data));
        }
    }
}

