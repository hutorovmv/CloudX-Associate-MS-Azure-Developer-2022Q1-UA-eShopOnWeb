using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using eShopOnWebFunctions.Services;

namespace eShopOnWebFunctions;

public class OrderItemsReserver
{
    private readonly ILogger<OrderItemsReserver> logger;

    private static string ServiceBusConnectionString => Environment.GetEnvironmentVariable("ServiceBusConnectionString");

    private static string StorageConnectionString => Environment.GetEnvironmentVariable("AzureWebJobsStorage");


    private static string ContainerName => Environment.GetEnvironmentVariable("OrdersContainerName");

    private static string TopicName => Environment.GetEnvironmentVariable("OrdersTopicName");

    private static string OrderBlobUploadFailureSubject => Environment.GetEnvironmentVariable("OrderBlobUploadFailureSubject");

    public OrderItemsReserver(ILogger<OrderItemsReserver> logger)
    {
        this.logger = logger;
    }

    [FunctionName("OrderItemsReserver")]
    public async Task Run([ServiceBusTrigger("orders", "order-upload", Connection = "ServiceBusConnectionString")]string message)
    {
        try
        {
            this.logger.LogInformation($"[OrderItemsReserver.Run] Trying to process order json to the blob container: {ContainerName} with connection string: {StorageConnectionString}...");

            // Error to test fallback Logic App
            // throw new Exception();

            var blobService = new BlobContainerService(StorageConnectionString, ContainerName, logger);
            var fileName = await blobService.Upload(message);

            this.logger.LogInformation($"The file: {fileName} is loaded successfully to the container: {ContainerName}");
        }
        catch (Exception ex)
        {
            this.logger.LogError($"[OrderItemsReserver.Run] Error on blob upload: {ex.Message}");
            var notificationService = new ServiceBusNotificationService(ServiceBusConnectionString, TopicName, this.logger);
            await notificationService.SendMessage(message, OrderBlobUploadFailureSubject);
        }
    }
}
