using System;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace OrderSubmitFunctions.Services;

internal class CosmosDbContainerService : IDisposable
{
    private readonly CosmosClient client;
    private readonly ILogger logger;

    private readonly string databaseName;
    private readonly string containerName;
    private readonly string partitionKeyPath;
    private readonly int throughput;
    
    public CosmosDbContainerService(
        string endpointUri,
        string endpointKey,
        ILogger logger,
        string databaseName,
        string containerName,
        string partitionKeyPath,
        int throughput = 10000
    )
    {
        this.logger = logger;

        this.databaseName = databaseName;
        this.containerName = containerName;
        this.partitionKeyPath = partitionKeyPath;
        this.throughput = throughput;

        this.client = new CosmosClient(endpointUri, endpointKey);
    }

    public async Task<ItemResponse<T>> CreateItemAsync<T>(T content, PartitionKey partitionKey)
    {
        var container = await this.GetContainerFromDatabaseAsync();
        return await container.CreateItemAsync<T>(content, partitionKey);
    }

    private async Task<Container> GetContainerFromDatabaseAsync()
    {
        var database = await this.GetDatabaseAsync();
        return await this.GetContainerAsync(database);
    }

    private async Task<Database> GetDatabaseAsync()
    {
        var databaseResponse = await this.client.CreateDatabaseAsync(this.databaseName);
        return databaseResponse.Database;
    }

    private async Task<Container> GetContainerAsync(Database database)
    {
        var containerProperties = new ContainerProperties(this.containerName, this.partitionKeyPath)
        {
            IndexingPolicy = GetIndexingPolicy()
        };
        var containerResponse = await database.CreateContainerIfNotExistsAsync(containerProperties, this.throughput);
        return containerResponse.Container;
    }

    private IndexingPolicy GetIndexingPolicy()
    {
        return new IndexingPolicy
        {
            IndexingMode = IndexingMode.Consistent,
            Automatic = true,
            IncludedPaths =
            {
                new IncludedPath
                {
                    Path = "/*"
                }
            }
        };
    }

    public void Dispose()
    {
        this.client.Dispose();
    }

    #region Alternative implementation of document creation
    //var documentClient = new DocumentClient(new Uri(endpoint), key);
    //try
    //{
    //  await documentClient.ReadDocumentAsync(
    //      UriFactory.CreateDocumentUri("eShopOnWeb", "Orders", $"${order.Id}"), 
    //      new RequestOptions
    //      {
    //          PartitionKey = new PartitionKey(order.Id)
    //      }
    //  );
    //}
    //catch (DocumentClientException de)
    //{
    //  if (de.StatusCode == HttpStatusCode.NotFound)
    //  {
    //      await documentClient.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri("eShopOnWeb", "Orders"), order);
    //  }
    //  else
    //  {
    //      return new ConflictObjectResult($"Record with id = {order.Id} already exists.");
    //  }
    //}
    //catch (Exception ex)
    //{
    //  return new JsonResult($"Message: {ex.Message}");
    //}
    #endregion
}
