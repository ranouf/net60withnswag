using Microsoft.Extensions.Logging;
using SK.CosmosDb.Models;
using SK.CosmosDB.Repositories;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace SK.CosmosDB.UnitOfWork
{
    public class UnitOfWork<TCosmosDatabase> : IUnitOfWork<TCosmosDatabase> where TCosmosDatabase : ICosmosDatabase
    {
        private Dictionary<Type, IRepository> _repositories;
        private readonly ILogger<UnitOfWork<TCosmosDatabase>> _logger;
        private readonly ILoggerFactory _loggerFactory;

        public ICosmosDatabase CosmosDatabase { get; private set; }

        public UnitOfWork(
            [NotNull] TCosmosDatabase database,
            ILogger<UnitOfWork<TCosmosDatabase>> logger,
            ILoggerFactory loggerFactory
        )
        {
            CosmosDatabase = database;
            _logger = logger;
            _loggerFactory = loggerFactory;
        }

        public async Task<(int success, int failed)> SaveChangesAsync()
        {
            var tasks = new List<Task<(int success, int failed)>>();
            foreach (var keyValuePair in _repositories)
            {
                tasks.AddRange(keyValuePair.Value.Tasks);
            }
            var results = await Task.WhenAll(tasks);
            return results.Aggregate((
                   (int success, int failed) total,
                   (int success, int failed) next
                ) =>
                {
                    return (
                        success: total.success + next.success,
                        failed: total.failed + next.failed
                    );
                });
        }

        IRepository<TEntity> IUnitOfWork.GetRepository<TEntity>()
        {
            if (_repositories == null)
            {
                _repositories = new Dictionary<Type, IRepository>();
            }

            var type = typeof(TEntity);
            if (!_repositories.ContainsKey(type))
            {
                var logger = _loggerFactory.CreateLogger<Repository<TEntity>>();
                _repositories[type] = new Repository<TEntity>(CosmosDatabase, logger);
            }

            return (IRepository<TEntity>)_repositories[type];
        }
    }
}
