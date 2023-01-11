using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AKExpensesTracker.Server.Functions.Services;

public class AzureBlobStorageService : IStorageServices
{
    private readonly string _connectionString;

    public AzureBlobStorageService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task DeleteFileAsync(string filePath)
    {
        var container = await GetContainerAsync();
        var fileName = Path.GetFileName(filePath);
        var blob = container.GetBlobClient(fileName);
        await blob.DeleteIfExistsAsync();
    }

    public async Task<string> SaveFileAsync(Stream stream, string fileName)
    {
        var container = await GetContainerAsync();

        // Retrieve and validate the extension
        var extension = Path.GetExtension(fileName).ToLower();

        // Create a unique name for the file
        var uniqueFileName = Path.GetFileNameWithoutExtension(fileName);
        var uniqueName = $"{uniqueFileName}-{Guid.NewGuid()}{extension}";

        var blob = container.GetBlobClient(uniqueName);
        await blob.UploadAsync(stream, new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders
            {
                ContentType = GetContentType(extension)
            }
        });

        return blob.Uri.AbsoluteUri;

    }

    private string GetContentType(string extension)
    {
        return extension switch
        {
            ".jpg" => "image/jpeg",
            ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            _ => throw new NotSupportedException($"The extension {extension} is not supported")
        };
    }

    private async Task<BlobContainerClient> GetContainerAsync()
    {
        var blobClient = new BlobServiceClient(_connectionString);
        var container = blobClient.GetBlobContainerClient("attachments");
        await container.CreateIfNotExistsAsync();
        return container;
    }
}

