using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace eShopOnWebFunctions.Models;

internal class AddressModel
{
    [JsonProperty("Street")]
    internal string Street { get; set; }

    [JsonProperty("State")]
    internal string State { get; set; }

    [JsonProperty("City")]
    internal string City { get; set; }

    [JsonProperty("Country")]
    internal string Country { get; set; }

    [JsonProperty("ZipCode")]
    internal string ZipCode { get; set; }
}
