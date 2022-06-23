using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using eShopOnWebFunctions.Services;

namespace eShopOnWebFunctions;

public static class OrderItemsBlobResolver
{
    private static string ConnectionString => Environment.GetEnvironmentVariable("AzureWebJobsStorage");

    private static string ContainerName => Environment.GetEnvironmentVariable("OrdersContainerName");

    [FunctionName("OrderItemsBlobResolver")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Admin, "post", Route = null)] HttpRequest req,
        ILogger logger)
    {
        try
        {
            logger.LogInformation($"[OrderItemsBlobResolver.Run] Trying to process order json to the blob container: {ContainerName} with connection string: {ConnectionString}...");

            var blobService = new BlobContainerService(ConnectionString, ContainerName, logger);
            var fileName = await blobService.Upload(req.Body);
            
            return new OkObjectResult($"The file: {fileName} is loaded successfully to the container: {ContainerName}");
        }
        catch (Exception ex)
        {
            logger.LogError($"[OrderItemsBlobResolver.Run] Error: {ex.Message}");
            return new NoContentResult();
        }
    }
}
