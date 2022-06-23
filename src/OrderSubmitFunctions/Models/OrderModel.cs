using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace eShopOnWebFunctions.Models;

internal class OrderModel
{
    public OrderModel()
    {
        this.DocumentId = Guid.NewGuid().ToString();
    }

    [JsonProperty("id")]
    internal string DocumentId { get; private set;  }

    [JsonProperty("Id")]
    internal int Id { get; set; }

    [JsonProperty("OrderItems")]
    internal List<OrderItemModel> OrderItems { get; set; }

    [JsonProperty("TotalPrice")]
    internal decimal TotalPrice { get; set; }

    [JsonProperty("ShipToAddress")]
    internal AddressModel ShipToAddress { get; set; }
}
