using Microsoft.eShopWeb.ApplicationCore.Models.Configuration;

namespace Microsoft.eShopWeb.Infrastructure.Models;

internal class OrderSubmitFunctionsConfigurationModel : FunctionsAppConfigurationModel
{
    public FunctionConfigurationModel OrderItemsBlobResolver { get; set; }

    public FunctionConfigurationModel OrderItemsCosmosDbResolver { get; set; }
}
