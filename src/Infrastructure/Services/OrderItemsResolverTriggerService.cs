using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.eShopWeb.Infrastructure.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Microsoft.eShopWeb.ApplicationCore.Services;

public class OrderItemsResolverTriggerService : IOrderItemsResolverTriggerService
{
    private readonly OrderSubmitFunctionsConfigurationModel configurationModel;
    private readonly HttpClient httpClient;

    private const string FunctionKeyHeaderName = "x-functions-key";

    public OrderItemsResolverTriggerService(IConfiguration configuration)
    {
        this.configurationModel = configuration.GetSection("OrderSubmitFunctions").Get<OrderSubmitFunctionsConfigurationModel>();

        this.httpClient = new HttpClient();
        this.httpClient.BaseAddress = new Uri(configurationModel.BaseUrl);
    }

    public async Task TriggerOrderItemsBlobResolver(Order order)
    {
        await this.TriggerHttpOrderItemsResolver(
            order,
            configurationModel.OrderItemsBlobResolver.Endpoint,
            configurationModel.OrderItemsBlobResolver.Key
        );
    }

    public async Task TriggerOrderItemsCosmosDbResolver(Order order)
    {
        await this.TriggerHttpOrderItemsResolver(
            order,
            configurationModel.OrderItemsCosmosDbResolver.Endpoint,
            configurationModel.OrderItemsCosmosDbResolver.Key
        );
    }

    private async Task TriggerHttpOrderItemsResolver(Order order, string endpointUrl, string key)
    {
        var orderJson = JsonConvert.SerializeObject(order);

        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, endpointUrl);
        httpRequestMessage.Headers.Add(FunctionKeyHeaderName, key);
        httpRequestMessage.Content = new StringContent(orderJson, Encoding.UTF8, "application/json");

        await this.httpClient.SendAsync(httpRequestMessage);
    }
}
