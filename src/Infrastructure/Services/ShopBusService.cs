using System;
using Azure.Messaging.ServiceBus;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.eShopWeb.ApplicationCore.Models.Configuration;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Microsoft.eShopWeb.Infrastructure.Services;

public class ShopBusService : IDisposable, IShopBusService
{
    private readonly ServiceBusConfigurationModel configurationModel;
    private readonly ServiceBusClient client;

    public ShopBusService(IConfiguration configuration)
    {
        this.configurationModel = configuration.GetSection("eShopBus").Get<ServiceBusConfigurationModel>();

        this.client = new ServiceBusClient(this.configurationModel.ConnectionString);
    }

    public async void EnqueueOrderForUpload(Order order)
    {
        var sender = client.CreateSender(configurationModel.OrdersTopic.Name);
        var orderJson = JsonConvert.SerializeObject(order);

        var message = new ServiceBusMessage(orderJson)
        {
            Subject = configurationModel.OrdersTopic.UploadSubscription.MessageSubject
        };
        await sender.SendMessageAsync(message);
    }

    public async void Dispose()
    {
        await this.client.DisposeAsync();
    }
}
