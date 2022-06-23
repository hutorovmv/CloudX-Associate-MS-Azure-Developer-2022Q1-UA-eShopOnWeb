using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;

namespace eShopOnWebFunctions.Services;

internal class ServiceBusNotificationService : IDisposable
{
    private readonly ServiceBusClient client;
    private readonly ServiceBusSender sender;
    private readonly ILogger logger;

    public ServiceBusNotificationService(
        string connectionString, 
        string topicName,
        ILogger logger
    )
    {
        this.logger = logger;

        this.logger.LogDebug($"[ServiceBusNotificationService] Trying to initialize service bus client with connection string: {connectionString}...");
        this.client = new ServiceBusClient(connectionString);

        this.logger.LogDebug($"[ServiceBusNotificationService] Trying to initialize service bus sender with topic: {topicName}...");
        this.sender = this.client.CreateSender(topicName);
    }

    public async Task SendMessage(string message, string subject)
    {
        this.logger.LogDebug($"[ServiceBusNotificationService.SendMessage] Trying to send message: {message} with subject: {subject}...");

        await this.sender.SendMessageAsync(new ServiceBusMessage(message)
        {
            Subject = subject
        });
    }

    public async void Dispose()
    {
        await this.client.DisposeAsync();
    }
}
