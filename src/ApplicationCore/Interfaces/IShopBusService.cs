using System;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;

namespace Microsoft.eShopWeb.ApplicationCore.Interfaces;

public interface IShopBusService : IDisposable
{
    void EnqueueOrderForUpload(Order order);
}
