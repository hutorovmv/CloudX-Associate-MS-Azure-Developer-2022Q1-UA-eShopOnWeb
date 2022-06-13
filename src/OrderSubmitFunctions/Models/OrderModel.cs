using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace OrderSubmitFunctions.Models;

internal class OrderModel
{
    public OrderModel()
    {
        this.DocumentId = Guid.NewGuid().ToString();
    }

    [JsonProperty("id")]
    internal string DocumentId { get; }

    [JsonProperty("OrderId")]
    internal int Id { get; set; }

    internal List<OrderItemModel> OrderItems { get; set; }

    internal decimal TotalPrice { get; set; }

    internal AddressModel ShipToAddress { get; set; }
}
