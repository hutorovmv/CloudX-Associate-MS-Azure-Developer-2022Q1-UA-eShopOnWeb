using Microsoft.eShopWeb.ApplicationCore.Models.Configuration;

namespace Microsoft.eShopWeb.Infrastructure.Models;

internal class EShopOnWebFunctionsConfigurationModel : FunctionsAppConfigurationModel
{
    public FunctionConfigurationModel OrderItemsBlobResolver { get; set; }

    public FunctionConfigurationModel DeliveryItemsProcessor { get; set; }
}
