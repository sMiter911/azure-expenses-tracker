using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AKExpensesTracker.Server.Data.Interfaces;
using AKExpensesTracker.Server.Functions.Services;
using AKExpensesTracker.Shared.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace AKExpensesTracker.Server.Functions
{
    public class UploadAttachment
    {
        private readonly ILogger<UploadAttachment> _logger;
        private readonly IStorageServices _storageServices;
        private readonly ICosmosAttachmentsRepository _attachmentsRepository;
        private readonly IImageAnalyzerService _imageAnalyzerService;

        public UploadAttachment(ILogger<UploadAttachment> log, 
            IStorageServices storageServices, 
            ICosmosAttachmentsRepository attachmentsRepository, 
            IImageAnalyzerService imageAnalyzerService)
        {
            _logger = log;
            _storageServices = storageServices;
            _attachmentsRepository = attachmentsRepository;
            _imageAnalyzerService = imageAnalyzerService;
        }

        [FunctionName("UploadAttachment")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("Upload attachment has been triggered");
            var userId = "userId";

            // Read the file from the form
            var file = req.Form.Files["File"];
            if (file == null)
            {
                return new BadRequestObjectResult(new ApiErrorResponse("File is required"));
            }

            // Computer vision processing
            var extension = Path.GetExtension(file.FileName);
            var validExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            if (!validExtensions.Contains(extension))
            {
                return new BadRequestObjectResult(new ApiErrorResponse("Invalid file type, please upload valid image file "));
            }

            using (var stream  = file.OpenReadStream())
            {
                var categories =  await _imageAnalyzerService.ExtractImageCategoriesAsync(stream);
                if(!categories.Any(c => c.StartsWith("paper") || c.StartsWith("text")))
                {
                    return new BadRequestObjectResult(new ApiErrorResponse("IMAGE NOT RELEVANT: Please upload a receipt or a document with text"));
                }
            }

                // Save the file and retrieve the URL
                string url = string.Empty;
            url = await _storageServices.SaveFileAsync(file.OpenReadStream(), file.FileName);

            // Save the URL in Cosmos DB
            await _attachmentsRepository.AddAsync(new Data.Models.Attachment
            {
                Id = Guid.NewGuid().ToString(),
                UploadedByUserId = userId,
                UploadedDate = DateTime.UtcNow,
                Url = url
            });

            return new OkObjectResult(new ApiSuccessResponse<string>($"Attachment has been uploaded", url));
        }
    }
}

