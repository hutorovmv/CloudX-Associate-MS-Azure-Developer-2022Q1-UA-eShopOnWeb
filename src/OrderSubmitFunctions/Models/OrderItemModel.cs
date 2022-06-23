using Newtonsoft.Json;

namespace eShopOnWebFunctions.Models;

internal class OrderItemModel
{
    [JsonProperty("Id")]
    internal int Id { get; set; }

    [JsonProperty("ItemOrdered")]
    internal CatalogItemOrderedModel ItemOrdered { get; set; }

    [JsonProperty("UnitPrice")]
    internal decimal UnitPrice { get; set; }

    [JsonProperty("Units")]
    internal int Units { get; set; }
}
