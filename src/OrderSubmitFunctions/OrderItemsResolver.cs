using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using OrderSubmitFunctions.Services;

namespace OrderSubmitFunctions;

public class OrderItemsResolver
{
    private readonly ILogger<OrderItemsResolver> logger;

    private string ConnectionString => Environment.GetEnvironmentVariable("ServiceBusConnectionString");

    private static string ContainerName => Environment.GetEnvironmentVariable("OrdersContainerName");

    private static string TopicName => Environment.GetEnvironmentVariable("OrdersTopicName");

    private static string OrderBlobUploadFailureSubject => Environment.GetEnvironmentVariable("OrderBlobUploadFailureSubject");

    public OrderItemsResolver(ILogger<OrderItemsResolver> logger)
    {
        this.logger = logger;
    }

    [FunctionName("OrderItemsResolver")]
    public async Task Run([ServiceBusTrigger("orders", "order-upload", Connection = "ServiceBusConnectionString")]string message)
    {
        try
        {
            this.logger.LogInformation($"[OrderItemsResolver.Run] Trying to process order json to the blob container: {ContainerName} with connection string: {ConnectionString}...");

            // Error to test fallback Logic App
            // throw new Exception();

            var blobService = new BlobContainerService(ConnectionString, ContainerName, logger);
            var fileName = await blobService.Upload(message);

            this.logger.LogInformation($"The file: {fileName} is loaded successfully to the container: {ContainerName}");
        }
        catch (Exception ex)
        {
            this.logger.LogError($"[OrderItemsResolver.Run] Error on blob upload: {ex.Message}");
            var notificationService = new ServiceBusNotificationService(ConnectionString, TopicName, this.logger);
            await notificationService.SendMessage(message, OrderBlobUploadFailureSubject);
        }
    }
}
