using Newtonsoft.Json;

namespace eShopOnWebFunctions.Models;

internal class CatalogItemOrderedModel
{
    [JsonProperty("CatalogItemId")]
    internal int CatalogItemId { get; set; }

    [JsonProperty("ProductName")]
    internal string ProductName { get; set; }

    [JsonProperty("PictureUri")]
    internal string PictureUri { get; set; }
}
