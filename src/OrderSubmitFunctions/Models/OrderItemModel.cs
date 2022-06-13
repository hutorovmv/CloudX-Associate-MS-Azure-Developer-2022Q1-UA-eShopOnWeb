namespace OrderSubmitFunctions.Models;

internal class OrderItemModel
{
    internal int Id { get; set; }

    internal CatalogItemOrderedModel ItemOrdered { get; set; }

    internal decimal UnitPrice { get; set; }

    internal int Units { get; set; }
}
