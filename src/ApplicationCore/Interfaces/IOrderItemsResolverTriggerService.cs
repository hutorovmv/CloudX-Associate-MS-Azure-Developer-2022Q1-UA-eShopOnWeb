using System.Threading.Tasks;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;

namespace Microsoft.eShopWeb.ApplicationCore.Interfaces;
public interface IOrderItemsResolverTriggerService
{
    Task TriggerOrderItemsBlobResolver(Order order);

    Task TriggerOrderItemsCosmosDbResolver(Order order);
}
