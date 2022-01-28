using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SK.CosmosDb.Exceptions;
using SK.CosmosDB.Configuration;
using SK.Entities;
using System.Net;
using System.Threading.Tasks;

namespace SK.CosmosDb.Models
{
    public abstract class CosmosDatabase : ICosmosDatabase
    {
        private readonly CosmosDBSettings _cosmosDBSettings;
        private readonly ILogger _logger;
        private CosmosClient _cosmosClient;
        public CosmosClient CosmosClient
        {
            get
            {
                if (_cosmosClient == null)
                {
                    _cosmosClient = new CosmosClient(
                        _cosmosDBSettings.Endpoint.AbsoluteUri,
                        _cosmosDBSettings.Key,
                        new CosmosClientOptions()
                        {
                            GatewayModeMaxConnectionLimit = _cosmosDBSettings.MaxConnectionLimit,
                            AllowBulkExecution = true
                        }
                    );
                }
                return _cosmosClient;
            }
        }

        public CosmosDatabase(
            IOptions<CosmosDBSettings> cosmosDBSettings,
            ILogger<CosmosDatabase> logger
        )
        {
            _cosmosDBSettings = cosmosDBSettings.Value;
            _logger = logger;
        }


        public async Task EnsureCreatedAsync()
        {
            await EnsureDatabaseExistsAsync();
            await EnsureContainersExistAsync();
        }

        private async Task EnsureContainersExistAsync()
        {
            foreach (var propertyInfo in GetType().GetProperties())
            {
                var propertyType = propertyInfo.PropertyType;
                if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition().Equals(typeof(CosmosContainer<>)))
                {
                    var value = propertyInfo.GetValue(this, null);
                    var name = (string)value.GetType().GetProperty("Name").GetValue(value);
                    var indexingPolicy = (IndexingPolicy)value.GetType().GetProperty(nameof(IndexingPolicy)).GetValue(value);
                    var container = await EnsureContainerExistsAsync(name, indexingPolicy);
                    value.GetType().GetProperty(nameof(Container)).SetValue(value, container);
                }
            }
        }

        public CosmosContainer<TEntity> GetCosmosContainer<TEntity>() where TEntity : IEntity<string>
        {
            foreach (var propertyInfo in GetType().GetProperties())
            {
                if (typeof(CosmosContainer<TEntity>).IsAssignableFrom(propertyInfo.PropertyType))
                {
                    return (CosmosContainer<TEntity>)propertyInfo.GetValue(this, null);
                }
            }
            throw new EntityNotFoundException();
        }

        #region Private
        private async Task<Container> EnsureContainerExistsAsync(string containerName, IndexingPolicy indexingPolicy)
        {
            var database = CosmosClient.GetDatabase(_cosmosDBSettings.DatabaseId);
            var response = await database.CreateContainerIfNotExistsAsync(
                new ContainerProperties(containerName, "/id") //TODO: PartitionKey should maybe customizable?
                {
                    IndexingPolicy = indexingPolicy
                },
                throughput: _cosmosDBSettings.Throughput
            );
            if (response.StatusCode == HttpStatusCode.Created)
            {
                _logger.LogInformation($"Container '{containerName}' has been created.");
            }
            else if (response.StatusCode == HttpStatusCode.OK)
            {
                _logger.LogInformation($"Container '{containerName}' already exists.");
            }

            return response.Container;
        }

        private async Task EnsureDatabaseExistsAsync()
        {
            var response = await CosmosClient.CreateDatabaseIfNotExistsAsync(
                _cosmosDBSettings.DatabaseId
            );
            if (response.StatusCode == HttpStatusCode.Created)
            {
                _logger.LogInformation($"Database '{_cosmosDBSettings.DatabaseId}' has been created.");
            }
            else if (response.StatusCode == HttpStatusCode.OK)
            {
                _logger.LogInformation($"Database '{_cosmosDBSettings.DatabaseId}' already exists.");
            }
        }
        #endregion
    }
}
