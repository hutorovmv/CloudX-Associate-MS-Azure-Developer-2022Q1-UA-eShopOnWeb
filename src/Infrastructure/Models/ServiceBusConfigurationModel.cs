using Microsoft.eShopWeb.Infrastructure.Models;

namespace Microsoft.eShopWeb.ApplicationCore.Models.Configuration;

internal class ServiceBusConfigurationModel
{
    public string ConnectionString { get; set; }

    public ServiceBusTopicConfigurationModel OrdersTopic { get; set; }
}
