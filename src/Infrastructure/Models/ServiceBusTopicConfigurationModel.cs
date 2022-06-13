using Microsoft.eShopWeb.ApplicationCore.Models.Configuration;

namespace Microsoft.eShopWeb.Infrastructure.Models;

internal class ServiceBusTopicConfigurationModel
{
    public string Name { get; set; }

    public ServiceBusSubscriptionFilterConfigurationModel UploadSubscription { get; set; }
}
