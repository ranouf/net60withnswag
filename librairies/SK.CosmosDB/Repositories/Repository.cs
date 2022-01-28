using Microsoft.Azure.Cosmos;
using SK.CosmosDb.Models;
using SK.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Linq;
using System.Linq;
using SK.CosmosDB.Extensions;
using Microsoft.Extensions.Logging;

namespace SK.CosmosDB.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : IEntity<string>
    {
        private Container Container
        {
            get
            {
                return CosmosDatabase.GetCosmosContainer<TEntity>().Container;
            }
        }
        private readonly ILogger _logger;

        public ICosmosDatabase CosmosDatabase { get; private set; }


        public IList<Task<(int success, int failed)>> Tasks { get; private set; } = new List<Task<(int success, int failed)>>();


        public Repository(
            [NotNull] ICosmosDatabase database,
            ILogger<Repository<TEntity>> logger
        )
        {
            CosmosDatabase = database;
            _logger = logger;
        }

        public IQueryable<TEntity> GetQuery()
        {
            var result = Container.GetItemLinqQueryable<TEntity>();
            return result;
        }

        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            var result = await Container.GetItemLinqQueryable<TEntity>()
                .ToFeedIterator()
                .ToAsyncEnumerable()
                .CountAsync();
            return result;
        }

        public async Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate)
        {
            try
            {
                var result = await Container.GetItemLinqQueryable<TEntity>()
                    .Where(predicate)
                    .ToFeedIterator()
                    .ToAsyncEnumerable()
                    .ToListAsync();
                return result;
            }
            catch (CosmosException e)
            {
                if (e.StatusCode == HttpStatusCode.NotFound)
                {
                    return default;
                }
                throw;
            }
        }

        public async Task<TEntity> FirstOrDefaultAsync(string id)
        {
            try
            {
                var result = await Container.GetItemLinqQueryable<TEntity>()
                    .Where(e => e.Id == id)
                    .ToFeedIterator()
                    .ToAsyncEnumerable()
                    .FirstOrDefaultAsync();
                return result;
            }
            catch (CosmosException e)
            {
                if (e.StatusCode == HttpStatusCode.NotFound)
                {
                    return default;
                }
                throw;
            }
        }

        public async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            try
            {
                var result = await Container.GetItemLinqQueryable<TEntity>()
                    .Where(predicate)
                    .ToFeedIterator()
                    .ToAsyncEnumerable()
                    .FirstOrDefaultAsync();
                return result;
            }
            catch (CosmosException e)
            {
                if (e.StatusCode == HttpStatusCode.NotFound)
                {
                    return default;
                }
                throw;
            }
        }

        public TEntity Insert(TEntity entity)
        {
            var task = Container
                .CreateItemAsync(entity, new PartitionKey(entity.Id))
                .ContinueWith(itemResponse =>
                {
                    if (itemResponse.IsCompletedSuccessfully)
                    {
                        entity = itemResponse.Result;
                        _logger.LogInformation($"Entity created in CosmosDb with success, Id:'{entity.Id}'", entity);
                        return (success: 1, failed: 0);
                    }
                    HandleExceptionAfterCosmosDbOperation(
                        entity,
                        itemResponse,
                        "Entity creation failed in CosmosDb with Error Message:'{0}', Status Code:'{1}'",
                        "Entity creation failed in CosmosDb with Exception: '{0}'"
                    );
                    return (success: 0, failed: 1);
                });
            Tasks.Add(task);
            return entity;
        }

        public TEntity Update(TEntity entity)
        {
            var task = Container
                .ReplaceItemAsync(entity, entity.Id, new PartitionKey(entity.Id))
                .ContinueWith(itemResponse =>
                {
                    if (itemResponse.IsCompletedSuccessfully)
                    {
                        entity = itemResponse.Result;
                        _logger.LogInformation($"Entity created in CosmosDb with success, Id:'{entity.Id}'", entity);
                        return (success: 1, failed: 0);
                    }
                    HandleExceptionAfterCosmosDbOperation(
                        entity,
                        itemResponse,
                        "Entity updatation failed in CosmosDb with Error Message:'{0}', Status Code:'{1}'",
                        "Entity updatation failed in CosmosDb with Exception: '{0}'"
                    );
                    return (success: 0, failed: 1);
                });
            Tasks.Add(task);
            return entity;
        }

        public void Delete(TEntity entity)
        {
            var task = Container
                .DeleteItemAsync<TEntity>(entity.Id, new PartitionKey(entity.Id))
                .ContinueWith(itemResponse =>
                {
                    if (itemResponse.IsCompletedSuccessfully)
                    {
                        entity = itemResponse.Result;
                        return (success: 1, failed: 0);
                    }
                    HandleExceptionAfterCosmosDbOperation(
                        entity,
                        itemResponse,
                        "Entity deletion failed in CosmosDb with Error Message:'{0}', Status Code:'{1}'",
                        "Entity deletion failed in CosmosDb with Exception: '{0}'"
                    );
                    return (success: 0, failed: 1);
                });
            Tasks.Add(task);
        }

        private void HandleExceptionAfterCosmosDbOperation(
            TEntity entity,
            Task<ItemResponse<TEntity>> itemResponse,
            string operationFailedWithCosmosDbException,
            string operationFailed
        )
        {
            var innerExceptions = itemResponse.Exception.Flatten();
            if (innerExceptions.InnerExceptions.FirstOrDefault(innerEx => innerEx is CosmosException) is CosmosException cosmosException)
            {
                _logger.LogError(
                    string.Format(
                        operationFailedWithCosmosDbException,
                        cosmosException.Message,
                        cosmosException.StatusCode
                    ),
                    entity,
                    cosmosException
                );
            }
            else
            {
                var exception = innerExceptions.InnerExceptions.FirstOrDefault();
                _logger.LogError(
                    string.Format(
                        operationFailed,
                        exception.Message
                    ),
                    entity,
                    exception
                );
            }
        }
    }
}
