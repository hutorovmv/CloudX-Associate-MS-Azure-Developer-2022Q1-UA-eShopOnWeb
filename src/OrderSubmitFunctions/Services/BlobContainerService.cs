using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;

namespace eShopOnWebFunctions.Services;

internal class BlobContainerService
{
    private readonly BlobServiceClient serviceClient;
    private readonly BlobContainerClient containerClient;
    
    private readonly ILogger logger;

    public BlobContainerService(
        string connectionString, 
        string containerName,
        ILogger logger,
        int maxRetriesCount = 3
    )
    {
        this.logger = logger;

        this.serviceClient = GetBlobServiceClient(connectionString, maxRetriesCount);
        this.containerClient = GetBlobContainerClient(containerName);
    }

    public BlobServiceClient GetBlobServiceClient(string connectionString, int maxRetriesCount)
    {
        this.logger.LogDebug($"[BlobContainerService] Creating service client...");
        
        var blobClientOptions = new BlobClientOptions();
        blobClientOptions.Retry.MaxRetries = maxRetriesCount;
        
        return new BlobServiceClient(connectionString, blobClientOptions);
    }

    public BlobContainerClient GetBlobContainerClient(string containerName)
    {
        this.logger.LogDebug($"[BlobContainerService] Creating container client...");
        return serviceClient.GetBlobContainerClient(containerName);
    }

    public async Task<string> Upload(Stream stream)
    {
        var fileName = GenerateTextFileName();

        this.logger.LogDebug($"[BlobContainerService.Upload] Trying to upload order file: {fileName} from a stream...");
        await this.Upload(stream, fileName);

        return fileName;
    }

    public async Task Upload(Stream stream, string fileName)
    {
        var streamContent = await new StreamReader(stream).ReadToEndAsync();        
        await this.Upload(streamContent, fileName);
    }

    public async Task<string> Upload(string text)
    {
        var fileName = GenerateTextFileName();

        this.logger.LogDebug($"[BlobContainerService.Upload] Trying to upload order file: {fileName} from a string...");
        await this.Upload(text, fileName);

        return fileName;
    }

    public async Task Upload(string text, string fileName)
    {
        var encodedBytes = Encoding.UTF8.GetBytes(text);
        await this.Upload(encodedBytes, fileName);
    }

    public async Task Upload(byte[] bytes, string fileName)
    {
        using (var memoryStream = new MemoryStream(bytes))
        {
            await this.Upload(memoryStream, fileName);
        }
    }

    public async Task Upload(MemoryStream memoryStream, string fileName)
    {
        await this.containerClient.CreateIfNotExistsAsync();
        var blobClient = this.containerClient.GetBlobClient(fileName);       
        await blobClient.UploadAsync(memoryStream);
    }

    private string GetTimestamp()
    {
        return new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString();
    }

    private string GenerateTextFileName()
    {
        return $"{GetTimestamp()}.txt";
    }
}
