using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using OrderSubmitFunctions.Services;
using OrderSubmitFunctions.Models;
using Microsoft.Azure.Cosmos;

namespace OrderSubmitFunctions;

public static class OrderItemsCosmosDbResolver
{
    private static string EndpointUri => Environment.GetEnvironmentVariable("EndpointUri");

    private static string EndpointKey => Environment.GetEnvironmentVariable("EndpointKey");

    private static string DatabaseName => Environment.GetEnvironmentVariable("DatabaseName");

    private static string ContainerName => Environment.GetEnvironmentVariable("ContainerName");

    private static string PartitionKeyPath => Environment.GetEnvironmentVariable("PartitionKeyPath");

    [FunctionName("OrderItemsCosmosDbResolver")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Admin, "post", Route = null)] HttpRequest req,
        ILogger logger)
    {
        try
        {
            logger.LogInformation($"[OrderItemsCosmosDbResolver.Run] Trying to process order json to cosmosdb container: {ContainerName} with endpointUrl: {EndpointUri}...");

            logger.LogDebug($"[OrderItemsCosmosDbResolver.Run] Trying to read request body and deserialize it...");
            var orderModel = await GetOrderFromStream(req.Body);

            var cosmosContainerService = new CosmosDbContainerService(
                EndpointUri,
                EndpointKey,
                logger,
                DatabaseName,
                ContainerName,
                PartitionKeyPath
            );
            await cosmosContainerService.CreateItemAsync<OrderModel>(orderModel, new PartitionKey(orderModel.Id));

            return new OkObjectResult($"Item is added successfuly to the container: {ContainerName}");
        }
        catch (Exception ex)
        {
            logger.LogError($"[OrderItemsCosmosDbResolver.Run] Error: {ex.Message}");
            return new NoContentResult();
        }
    }

    private static async Task<OrderModel> GetOrderFromStream(Stream stream)
    {
        var contentJson = await new StreamReader(stream).ReadToEndAsync();
        return JsonConvert.DeserializeObject<OrderModel>(contentJson);
    }
}
